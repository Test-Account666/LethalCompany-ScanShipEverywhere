using System;
using System.Reflection;
using HarmonyLib;
using ScanTweaks;

namespace ScanShipEverywhere.Patches;

[HarmonyPatch(typeof(PingScan))]
public static class ScanTweaksPingScanPatch {
    private static FieldInfo? _rangeField;
    private static PingScan? _pingScan;

    [HarmonyPatch("OnEnable")]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void OnEnablePostfix(PingScan __instance) {
        _pingScan = __instance;

        SetValue();

        ScanShipEverywhere.configManager.SubscribeToMaxScanDistanceHardLimit(OnValueChanged);
    }

    [HarmonyPatch("OnDisable")]
    [HarmonyPostfix]
    private static void OnDisablePostfix() => ScanShipEverywhere.configManager.UnsubscribeFromMaxScanDistanceHardLimit(OnValueChanged);

    private static void OnValueChanged(object sender, EventArgs e) => SetValue();

    private static void SetValue() {
        if (_pingScan is null) return;

        _rangeField ??= AccessTools.DeclaredField(typeof(PingScan), "_range");

        if (_rangeField is null) {
            ScanShipEverywhere.Logger.LogFatal("Couldn't find \"_range\" field in ScanTweaks' \"PingScan\" type!'");
            return;
        }

        _rangeField.SetValue(_pingScan, ScanShipEverywhere.configManager.GetMaxScanDistanceHardLimit());
    }
}