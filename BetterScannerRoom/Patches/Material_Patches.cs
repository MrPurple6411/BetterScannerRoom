namespace BetterScannerRoom.Patches;

using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(Material))]
internal class Material_Patches
{
    [HarmonyPatch(nameof(Material.SetFloat), new[] { typeof(int), typeof(float) })]
    [HarmonyPrefix]
    public static void SetFloat_Prefix(int nameID, ref float value)
    {
        if (nameID == ShaderPropertyID._ScanFrequency)
            value = value > 1f ? 1f : value;

        if (!BSRSettings.MapPulseEnabled && (nameID == ShaderPropertyID._ScanFrequency || nameID == ShaderPropertyID._ScanIntensity))
            value = 0f;
    }
}
