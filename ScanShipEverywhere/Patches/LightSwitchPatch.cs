using HarmonyLib;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ScanShipEverywhere.Patches;

[HarmonyPatch(typeof(AutoParentToShip))]
public static class LightSwitchPatch {
    [HarmonyPatch(nameof(AutoParentToShip.Awake))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    private static void ConstructorPostfix(AutoParentToShip __instance) {
        var objectName = __instance?.name ?? "";

        if (!objectName.Equals("LightSwitchContainer"))
            return;

        Debug.Assert(__instance != null, nameof(__instance) + " != null");

        if (__instance.transform.Find("ScanNode") != null) {
            ScanShipEverywhere.Logger.LogDebug("Found a ScanNode already attached to LightSwitchContainer!");
            return;
        }

        CreateScanNodeOnObject(__instance.gameObject, ScanShipEverywhere.configManager.GetLightSwitchScanNodeHeaderText(),
                               ScanShipEverywhere.configManager.GetLightSwitchScanNodeSubText());

        ScanShipEverywhere.Logger.LogInfo("Added ScanNode to Light Switch!");
    }

    private static void CreateScanNodeOnObject(GameObject gameObject, string headerText, string? subText) {
        const int nodeType = 0;
        const int minRange = 0;
        const int maxRange = 6;
        const int size = 1;
        const int creatureScanID = -1;
        const int scrapValue = 0;
        const bool requiresLineOfSight = false;

        var scanNodeObject = new GameObject("ScanNode", typeof(ScanNodeProperties), typeof(BoxCollider), typeof(LightSwitchUpdater)) {
            layer = LayerMask.NameToLayer("ScanNode"),
            transform = {
                localScale = Vector3.one * size,
                parent = gameObject.transform,
            },
        };

        scanNodeObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        var scanNode = scanNodeObject.GetComponent<ScanNodeProperties>();

        scanNode.scrapValue = scrapValue;
        scanNode.creatureScanID = creatureScanID;
        scanNode.nodeType = nodeType;
        scanNode.minRange = minRange;
        scanNode.maxRange = maxRange;
        scanNode.requiresLineOfSight = requiresLineOfSight;
        scanNode.headerText = headerText;

        if (subText is null)
            return;

        scanNode.subText = subText;
    }
}