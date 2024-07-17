using HarmonyLib;
using UnityEngine;

namespace ScanShipEverywhere.Patches;

[HarmonyPatch(typeof(RoundManager))]
public static class RoundManagerPatch {
    [HarmonyPatch(nameof(RoundManager.FinishGeneratingLevel))]
    [HarmonyPostfix]
    private static void AttachUpdaterToScanNodes() {
        var allScanNodes = Object.FindObjectsOfType<ScanNodeProperties>();

        allScanNodes ??= [
        ];

        foreach (var scanNodeProperties in allScanNodes) {
            if (scanNodeProperties is null) continue;

            var scanNodeUpdater = scanNodeProperties.gameObject.AddComponent<ScanNodeUpdater>();

            scanNodeUpdater.SetScanNodeProperties(scanNodeProperties);

            if (scanNodeUpdater.IsValid()) continue;

            Object.Destroy(scanNodeUpdater);
        }
    }
}