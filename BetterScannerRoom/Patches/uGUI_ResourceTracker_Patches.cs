namespace BetterScannerRoom.Patches;

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

[HarmonyPatch(typeof(uGUI_ResourceTracker))]
internal class uGUI_ResourceTracker_Patches
{
    [HarmonyPatch(nameof(uGUI_ResourceTracker.GatherAll))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> GatherAll_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode.Equals(OpCodes.Ldc_R4) && instruction.operand.Equals(500f))
            {
                yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerBlipRange)));
                continue;
            }

            yield return instruction;
        }
    }

    [HarmonyPatch(nameof(uGUI_ResourceTracker.GatherScanned))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> GatherScanned_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode.Equals(OpCodes.Ldc_R4) && instruction.operand.Equals(500f))
            {
                yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerBlipRange)));
                continue;
            }

            yield return instruction;
        }
    }
}
