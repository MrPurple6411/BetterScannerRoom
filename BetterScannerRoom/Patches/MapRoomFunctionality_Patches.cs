namespace BetterScannerRoom.Patches;

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

[HarmonyPatch(typeof(MapRoomFunctionality))]
public static class MapRoomFunctionality_Patches
{
    [HarmonyPatch(nameof(MapRoomFunctionality.Start))]
    [HarmonyPostfix]
    public static void Start_Postfix(MapRoomFunctionality __instance)
    {
        BSRSettings.MapRooms.Add(__instance);
    }

    [HarmonyPatch(nameof(MapRoomFunctionality.mapScale), MethodType.Getter)]
    [HarmonyPostfix]
    public static void GetMapScale_Postfix(MapRoomFunctionality __instance, ref float __result)
    {
        __instance.UpdateScanRangeAndInterval();
        __result = __instance.hologramRadius / __instance.scanRange;
    }

    [HarmonyPatch(nameof(MapRoomFunctionality.UpdateScanRangeAndInterval))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> UpdateScanRangeAndInterval_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int succeeded = 0;

        foreach (var instruction in instructions)
        {
            if (instruction.opcode.Equals(OpCodes.Ldc_R4))
            {
                if (instruction.operand.Equals(500f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerMaxRange)));
                    succeeded++;
                    continue;
                }

                if (instruction.operand.Equals(300f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerDefaultRange)));
                    succeeded++;
                    continue;
                }

                if (instruction.operand.Equals(50f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerUpgradeAddedRange)));
                    succeeded++;
                    continue;
                }

                if (instruction.operand.Equals(14f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerSpeedDefaultInterval)));
                    succeeded++;
                    continue;
                }

                if (instruction.operand.Equals(3f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerSpeedIntervalPerModule)));
                    succeeded++;
                    continue;
                }
            }

            yield return instruction;
            continue;
        }

        Plugin.Logger.LogDebug($"{succeeded}/5 Transpilers succeeded.");
    }

    [HarmonyPatch(nameof(MapRoomFunctionality.UpdateScanRangeAndInterval))]
    [HarmonyPrefix]
    public static void UpdateScanRangeAndInterval_Prefix(MapRoomFunctionality __instance)
    {
        if(__instance.containerIsDirty)
            __instance.miniWorld.ClearAllChunks();
    }

    [HarmonyPatch(nameof(MapRoomFunctionality.UpdateScanRangeAndInterval))]
    [HarmonyPostfix]
    public static void UpdateScanRangeAndInterval_Postfix(MapRoomFunctionality __instance)
    {
        __instance.miniWorld.mapWorldRadius = Mathf.CeilToInt(__instance.scanRange);
    }

    [HarmonyPatch(nameof(MapRoomFunctionality.UpdateBlips))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> UpdateBlips_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int succeeded = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.opcode != OpCodes.Call || (MethodInfo)instruction.operand != AccessTools.Method(typeof(Mathf), nameof(Mathf.Min), new[] { typeof(int), typeof(int) }))
            {
                yield return instruction;
                continue;
            }

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MapRoomFunctionality_Patches), nameof(GetBlipCount)));
            //yield return instruction;
            succeeded++;
        }

        Plugin.Logger.LogDebug($"{succeeded}/1 Transpilers succeeded.");
    }

    public static int GetBlipCount(int value1, int value2, MapRoomFunctionality __instance)
    {
        int count = __instance.scanInterval <= 1f ? value2 : Mathf.Min(value1, value2);
        return count;
    }
}
