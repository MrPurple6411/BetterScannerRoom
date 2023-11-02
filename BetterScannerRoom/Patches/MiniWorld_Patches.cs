namespace BetterScannerRoom.Patches;

using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

[HarmonyPatch(typeof(MiniWorld))]
internal class MiniWorld_Patches
{
    private static readonly Dictionary<MiniWorld, MapRoomFunctionality> miniWorldMapRoomFunctionality = new();

    [HarmonyPatch(nameof(MiniWorld.Start))]
    [HarmonyPrefix]
    public static void GetScanRange_Prefix(MiniWorld __instance)
    {
        if (__instance.GetComponentInParent<MapRoomFunctionality>() is not MapRoomFunctionality mapRoomFunctionality)
            return;

        miniWorldMapRoomFunctionality[__instance] = mapRoomFunctionality;
    }

    [HarmonyPatch(nameof(MiniWorld.mapScale), MethodType.Getter)]
    [HarmonyPostfix]
    public static void GetMapScale_Postfix(MiniWorld __instance, ref float __result)
    {
        if (!miniWorldMapRoomFunctionality.TryGetValue(__instance, out MapRoomFunctionality mapRoomFunctionality))
            return;

        mapRoomFunctionality.UpdateScanRangeAndInterval();
        __result = __instance.hologramRadius / mapRoomFunctionality.scanRange;
    }

    [HarmonyPatch(nameof(MiniWorld.GetOrMakeChunk))]
    [HarmonyPrefix]
    public static void GetOrMakeChunk_Prefix(MiniWorld __instance)
    {
        __instance.chunkScale = __instance.mapScale * 1f;
    }

    [HarmonyPatch(nameof(MiniWorld.UpdatePosition))]
    [HarmonyPrefix]
    public static bool UpdatePosition_Prefix(MiniWorld __instance)
    {
        try
        {
            __instance.chunkScale = __instance.mapScale * 1f;
            __instance.hologramHolder.rotation = Quaternion.identity;
            __instance.materialInstance.SetVector(ShaderPropertyID._MapCenterWorldPos, __instance.transform.position);
            Vector3 vector = LargeWorldStreamer.main.land.transform.InverseTransformPoint(__instance.transform.position) / 4f;
            foreach (var keyValuePair in __instance.loadedChunks)
            {
                var value = keyValuePair.Value;
                Vector3 vector2 = (keyValuePair.Key * 32).ToVector3() - vector;
                value.gameObject.transform.localPosition = vector2 * __instance.chunkScale;
                value.gameObject.transform.localScale = Vector3.one * __instance.chunkScale;
            }
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"UpdatePosition_Prefix failed: {e}");
        }
        return false;
    }

    [HarmonyPatch(nameof(MiniWorld.GetOrMakeChunk))]
    [HarmonyPostfix]
    public static void GetOrMakeChunk_Postfix(MiniWorld __instance, Int3 chunkId)
    {
        if(__instance.loadedChunks.TryGetValue(chunkId, out var chunk))
        {
            __instance.chunkScale = __instance.mapScale * 1f;
            __instance.hologramHolder.rotation = Quaternion.identity;
            __instance.materialInstance.SetVector(ShaderPropertyID._MapCenterWorldPos, __instance.transform.position);
            Vector3 vector = LargeWorldStreamer.main.land.transform.InverseTransformPoint(__instance.transform.position) / 4f;
            Vector3 vector2 = (chunkId * 32).ToVector3() - vector;
            chunk.gameObject.transform.localPosition = vector2 * __instance.chunkScale;
            chunk.gameObject.transform.localScale = Vector3.one * __instance.chunkScale;
        }
    }

    [HarmonyPatch(nameof(MiniWorld.RebuildHologram))]
    [HarmonyPrefix]
    public static bool RebuildHologram_Prefix(MiniWorld __instance, ref IEnumerator __result)
    {
        __result = RebuildHologram_Replacement(__instance);
        return false;
    }

    private static IEnumerator RebuildHologram_Replacement(MiniWorld miniWorld)
    {
        bool isPickupable = miniWorld.GetComponentInParent<Pickupable>() != null;
        var handles = new List<AsyncOperationHandle<Mesh>>();
        while (miniWorld != null)
        {
            if (!miniWorld.gameObject.activeInHierarchy || (isPickupable && miniWorld.GetComponentInParent<Player>() == null))
            {
                miniWorld.ClearAllChunks();
            }
            else if (miniWorld.gameObject.activeInHierarchy)
            {
                Int3 block = LargeWorldStreamer.main.GetBlock(miniWorld.transform.position);
                Int3 @int = block - miniWorld.mapWorldRadius;
                Int3 int2 = block + miniWorld.mapWorldRadius;
                Int3 int3 = (@int >> 2) / 32;
                Int3 int4 = (int2 >> 2) / 32;
                bool chunkAdded = false;
                Int3.RangeEnumerator iter = Int3.Range(int3, int4);
                while (iter.MoveNext())
                {
                    Int3 chunkId = iter.Current;
                    miniWorld.requestChunks.Add(chunkId);
                    if (!miniWorld.GetChunkExists(chunkId))
                    {
                        string chunkPath = miniWorld.GetChunkFilename(chunkId);
                        if (!AddressablesUtility.Exists<Mesh>(chunkPath))
                            continue;

                        AsyncOperationHandle<Mesh> request = AddressablesUtility.LoadAsync<Mesh>(chunkPath);
                        request.Completed += (AsyncOperationHandle <Mesh> request)=>
                        {
                            handles.Remove(request);
                            if (request.Status == AsyncOperationStatus.Failed)
                                return;

                            if (miniWorld == null || miniWorld.GetChunkExists(chunkId))
                            {
                                AddressablesUtility.QueueRelease<Mesh>(ref request);
                                return;
                            }

#if SUBNAUTICA
                            miniWorld.GetOrMakeChunk(chunkId, request, chunkPath);
#else
                            miniWorld.GetOrMakeChunk(chunkId, request.Result, chunkPath);
#endif
                            chunkAdded = true;
                        };
                        handles.Add(request);
                    }
                }

                var list = new List<AsyncOperationHandle<Mesh>>(handles);
                foreach (var handle in list)
                    yield return handle;

                miniWorld.ClearUnusedChunks(miniWorld.requestChunks);
                miniWorld.requestChunks.Clear();
                if (chunkAdded)
                {
                    miniWorld.UpdatePosition();
                }
            }
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }
}
