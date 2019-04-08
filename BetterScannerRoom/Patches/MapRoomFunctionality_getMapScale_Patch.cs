using Harmony;

namespace BetterScannerRoom.Patches
{
    [HarmonyPatch(typeof(MapRoomFunctionality))]
    [HarmonyPatch("mapScale", PropertyMethod.Getter)]
    class MapRoomFunctionality_getMapScale_Patch
    {
        public static bool Prefix(MapRoomFunctionality __instance, ref float __result)
        {
            __result = __instance.hologramRadius / BSRSettings.Instance.ScannerBlipRange;
            return false;
        }
    }
}
