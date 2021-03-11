using HarmonyLib;
using System.Reflection;
using QModManager.API.ModLoading;

namespace BetterScannerRoom
{
    [QModCore]
    public static class BSRPatch
    {
        [QModPatch]
        public static void Patch()
        {
            BSRSettings.Load();
            
            Harmony harmony = new Harmony("betterscannerroom.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
