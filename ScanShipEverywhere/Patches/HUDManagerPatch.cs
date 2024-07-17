using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace ScanShipEverywhere.Patches;

[HarmonyPatch(typeof(HUDManager))]
public static class HUDManagerPatch {
    [HarmonyPatch(nameof(HUDManager.AssignNewNodes))]
    [HarmonyTranspiler]
    // This transpiler tries to increase the distance hard limit for node scans
    private static IEnumerable<CodeInstruction> AssignNewNodesTranspiler(IEnumerable<CodeInstruction> instructions) {
        var skipped = 0;

        foreach (var codeInstruction in instructions) {
            if (codeInstruction.opcode != OpCodes.Ldc_R4) {
                yield return codeInstruction;
                continue;
            }

            if (codeInstruction.operand is not float) {
                yield return codeInstruction;
                continue;
            }

            // The first two float values are not the one we're looking for.
            // We're looking for the one that sets the distance hard limit.
            // The vanilla value is 80F, which is unique in Zeekerss' code, but if another mod modifies it, a direct comparison would fail
            if (skipped < 2) {
                skipped += 1;
                yield return codeInstruction;
                continue;
            }

            yield return new(OpCodes.Ldc_R4, ScanShipEverywhere.configManager.GetMaxScanDistanceHardLimit());
            ScanShipEverywhere.Logger.LogDebug("Found max scan range!");
        }
    }
}