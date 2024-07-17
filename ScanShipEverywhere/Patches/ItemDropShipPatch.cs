using HarmonyLib;
using UnityEngine;

namespace ScanShipEverywhere.Patches;

[HarmonyPatch(typeof(ItemDropship))]
public static class ItemDropShipPatch {
    [HarmonyPatch(nameof(ItemDropship.Start))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    private static void StartPostfix(ItemDropship __instance) {
        if (!ScanShipEverywhere.configManager.AddDropShipScanNode()) return;

        var itemDropShipObject = __instance.gameObject;

        if (itemDropShipObject.transform.Find("ScanNode") != null) {
            ScanShipEverywhere.Logger.LogDebug("Found a ScanNode already attached to dropship!");
            return;
        }

        ScanShipEverywhere.Logger.LogDebug("Adding ScanNode to dropship!");

        var scanNodeHeaderText = ScanShipEverywhere.configManager.GetDropShipScanNodeHeaderText();

        var scanNodeSubText = ScanShipEverywhere.configManager.GetDropShipScanNodeSubText();

        CreateScanNodeOnObject(itemDropShipObject, scanNodeHeaderText, scanNodeSubText,
                               ScanShipEverywhere.configManager.GetMaxDropShipDistance());
    }

    private static void CreateScanNodeOnObject(GameObject gameObject, string headerText, string? subText, int maxRange) {
        const int nodeType = 0;
        const int minRange = 1;
        const int size = 4;

        var scanNodeObject = new GameObject("ScanNode", typeof(ScanNodeProperties), typeof(BoxCollider)) {
            layer = LayerMask.NameToLayer("ScanNode"),
            transform = {
                localScale = Vector3.one * size,
                parent = gameObject.transform,
            },
        };

        scanNodeObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        var scanNode = scanNodeObject.GetComponent<ScanNodeProperties>();

        scanNode.scrapValue = 0;
        scanNode.creatureScanID = -1;
        scanNode.nodeType = nodeType;
        scanNode.minRange = minRange;
        scanNode.maxRange = maxRange;
        scanNode.requiresLineOfSight = false;
        scanNode.headerText = headerText;

        if (subText is null) return;

        scanNode.subText = subText;
    }
}