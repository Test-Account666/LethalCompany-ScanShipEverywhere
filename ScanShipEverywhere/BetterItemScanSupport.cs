using BetterItemScan.Patches;
using HarmonyLib;

namespace ScanShipEverywhere;

public static class BetterItemScanSupport {
    public static void Initialize() {
        var maxRangeField = AccessTools.DeclaredField(typeof(PlayerControllerBPatch_A), "maxDistance");

        maxRangeField.SetValue(null, ScanShipEverywhere.configManager.GetMaxScanDistanceHardLimit());
    }
}