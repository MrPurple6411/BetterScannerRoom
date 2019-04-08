using Harmony;
using UnityEngine;

namespace BetterScannerRoom.Patches
{
    [HarmonyPatch(typeof(MapRoomFunctionality))]
    [HarmonyPatch("GetScanRange")]
    class MapRoomFunctionality_GetScanRange_Patch
    {
        public static bool Prefix(MapRoomFunctionality __instance, ref float __result)
        {
            __result = Mathf.Min(BSRSettings.Instance.ScannerBlipRange,
                BSRSettings.Instance.ScannerMinRange +
                (float) __instance.storageContainer.container.GetCount(TechType.MapRoomUpgradeScanRange) *
                BSRSettings.Instance.ScannerUpgradeAddedRange);
            return false;
        }
    }
}
