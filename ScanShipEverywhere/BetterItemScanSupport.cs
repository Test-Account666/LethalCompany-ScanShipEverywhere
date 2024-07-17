using System;
using System.Reflection;
using BetterItemScan.Patches;
using HarmonyLib;

namespace ScanShipEverywhere;

public static class BetterItemScanSupport {
    private static FieldInfo? _maxRangeField;

    public static void Initialize() {
        SetValue();

        ScanShipEverywhere.configManager.SubscribeToMaxScanDistanceHardLimit(OnValueChanged);
    }

    public static void Uninitialize() => ScanShipEverywhere.configManager.UnsubscribeFromMaxScanDistanceHardLimit(OnValueChanged);

    private static void OnValueChanged(object sender, EventArgs e) => SetValue();

    private static void SetValue() {
        _maxRangeField ??= AccessTools.DeclaredField(typeof(PlayerControllerBPatch_A), "maxDistance");

        if (_maxRangeField is null) {
            ScanShipEverywhere.Logger.LogFatal(
                "Couldn't find \"_maxRangeField\" field in BetterItemScan's \"PingScan\" PlayerControllerBPatch_A!'");
            return;
        }

        _maxRangeField.SetValue(null, ScanShipEverywhere.configManager.GetMaxScanDistanceHardLimit());
    }
}