namespace BetterScannerRoom.Patches;

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

[HarmonyPatch(typeof(uGUI_CameraDrone))]
internal class uGUI_CameraDrone_Patches
{
    [HarmonyPatch(nameof(uGUI_CameraDrone.LateUpdate))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> LateUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode.Equals(OpCodes.Ldc_R4))
            {
                if (instruction.operand.Equals(250f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.ScannerCameraRange)));
                    continue;
                }

                if (instruction.operand.Equals(520f))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(BSRSettings).GetMethod(nameof(BSRSettings.GetExtendedCameraRange)));
                    continue;
                }
            }

            yield return instruction;
        }
    }
}
