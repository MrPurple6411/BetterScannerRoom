using Harmony;
using UnityEngine;

namespace BetterScannerRoom.Patches
{
    [HarmonyPatch(typeof(MapRoomFunctionality))]
    [HarmonyPatch("GetScanInterval")]
    class MapRoomFunctionality_GetScanInterval_Patch
    {
        public static bool Prefix(MapRoomFunctionality __instance, ref float __result)
        {
            __result = Mathf.Max(BSRSettings.Instance.ScannerSpeedMinimumInterval, BSRSettings.Instance.ScannerSpeedNormalInterval - __instance.storageContainer.container.GetCount(TechType.MapRoomUpgradeScanSpeed) * BSRSettings.Instance.ScannerSpeedIntervalPerModule);
            return false;
        }
    }
}
