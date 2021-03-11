using System;
using HarmonyLib;
using System.Reflection;

namespace BetterScannerRoom
{
    class BSRPatch
    {
        public static void Patch()
        {
            BSRSettings.Load();
            
            Harmony harmony = new Harmony("betterscannerroom.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
