#if BZ
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using QModManager.Utility;

namespace BetterScannerRoom.Patches
{
    [HarmonyPatch(typeof(MapRoomFunctionality), nameof(MapRoomFunctionality.UpdateScanRangeAndInterval))]
    class MapRoomFunctionality_UpdateScanRangeAndInterval_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int succeeded = 0;
            
            foreach (var instruction in instructions)
            {
                if (instruction.opcode.Equals(OpCodes.Ldc_R4))
                {
                    if (instruction.operand.Equals(500f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, BSRSettings.Instance.ScannerBlipRange);
                        succeeded++;
                        continue;
                    }

                    if (instruction.operand.Equals(300f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, BSRSettings.Instance.ScannerMinRange);
                        succeeded++;
                        continue;
                    }

                    if (instruction.operand.Equals(50f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, BSRSettings.Instance.ScannerUpgradeAddedRange);
                        succeeded++;
                        continue;
                    }
                    
                    if (instruction.operand.Equals(1f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, BSRSettings.Instance.ScannerSpeedMinimumInterval);
                        succeeded++;
                        continue;
                    }

                    if (instruction.operand.Equals(14f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, BSRSettings.Instance.ScannerSpeedNormalInterval);
                        succeeded++;
                        continue;
                    }

                    if (instruction.operand.Equals(3f))
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_R4, BSRSettings.Instance.ScannerSpeedIntervalPerModule);
                        succeeded++;
                        continue;
                    }
                }

                yield return instruction;
            }

            Logger.Log(Logger.Level.Info, $"{succeeded}/6 Transpilers succeeded.");
        }
    }
}
#endif