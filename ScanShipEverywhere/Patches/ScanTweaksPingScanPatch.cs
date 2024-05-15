using HarmonyLib;
using ScanTweaks;

namespace ScanShipEverywhere.Patches;

[HarmonyPatch(typeof(PingScan))]
public static class ScanTweaksPingScanPatch {
    [HarmonyPatch("OnEnable")]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void OnEnablePostfix(PingScan __instance) {
        var rangeField = AccessTools.DeclaredField(typeof(PingScan), "_range");

        if (rangeField is null) {
            ScanShipEverywhere.Logger.LogFatal("Couldn't find \"_range\" field in ScanTweaks' \"PingScan\" type!'");
            return;
        }

        rangeField.SetValue(__instance, ScanShipEverywhere.configManager.GetMaxScanDistanceHardLimit());
    }
}