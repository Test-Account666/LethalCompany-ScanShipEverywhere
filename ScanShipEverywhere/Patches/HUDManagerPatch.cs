using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

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

    [HarmonyPatch(nameof(HUDManager.AttemptScanNode))]
    [HarmonyPostfix]
    private static void AttemptScanNode(ScanNodeProperties node, PlayerControllerB playerScript) {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (node is null)
            return;

        if (StartOfRound.Instance is null)
            return;

        if (playerScript != StartOfRound.Instance.localPlayerController)
            return;


        var scanNodeObject = node.gameObject;

        var scanNodeParentObject = scanNodeObject.transform.parent.gameObject;

        // Increase/Decrease the maxRange for specific scan nodes

        HandleDropShip(playerScript, node, scanNodeParentObject);

        HandleEntrance(playerScript, node, scanNodeParentObject);

        HandleSpaceShip(playerScript, node, scanNodeParentObject);
    }

    private static void HandleEntrance(PlayerControllerB playerControllerB, ScanNodeProperties scanNodeProperties,
                                       GameObject entranceTeleportObject) {
        var maxRange = ScanShipEverywhere.configManager.GetMaxEntranceDistance();

        // The main entrance scan node is not bound to the actual entrance in vanilla maps... For some reason ¯\_(ツ)_/¯
        if (scanNodeProperties.headerText.ToLower().Contains("entrance")) {
            if (playerControllerB.isInsideFactory)
                maxRange = 2;

            UpdateScanNode(scanNodeProperties, false, 1, maxRange);
        }

        // In case of custom maps that might add the scan node to the actual thing

        var entranceTeleport = entranceTeleportObject.GetComponent<EntranceTeleport>();

        if (entranceTeleport is null)
            return;

        maxRange = ScanShipEverywhere.configManager.GetMaxEntranceDistance();

        switch (entranceTeleport.isEntranceToBuilding) {
            case true when playerControllerB.isInsideFactory:
            case false when !playerControllerB.isInsideFactory:
                maxRange = 2;
                break;
        }

        UpdateScanNode(scanNodeProperties, false, 1, maxRange);
    }

    private static void HandleDropShip(PlayerControllerB playerControllerB, ScanNodeProperties scanNodeProperties,
                                       GameObject dropShipObject) {
        var itemDropShip = dropShipObject.GetComponent<ItemDropship>();

        if (itemDropShip is null)
            return;

        var maxRange = itemDropShip.deliveringOrder? ScanShipEverywhere.configManager.GetMaxDropShipDistance() : 2;

        if (playerControllerB.isInsideFactory)
            maxRange = 2;

        UpdateScanNode(scanNodeProperties, false, 1, maxRange);
    }

    private static void HandleSpaceShip(PlayerControllerB playerControllerB, ScanNodeProperties scanNodeProperties,
                                        GameObject spaceShipObject) {
        var terminal = spaceShipObject.GetComponent<Terminal>();

        if (terminal is null)
            return;

        var maxRange = ScanShipEverywhere.configManager.GetMaxShipDistance();

        if (playerControllerB.isInsideFactory || playerControllerB.isInHangarShipRoom)
            maxRange = 2;

        UpdateScanNode(scanNodeProperties, false, 1, maxRange);
    }

    private static void UpdateScanNode(ScanNodeProperties scanNodeProperties, bool requiresLineOfSight, int minRange, int maxRange) {
        scanNodeProperties.requiresLineOfSight = requiresLineOfSight;

        scanNodeProperties.minRange = minRange;
        scanNodeProperties.maxRange = maxRange;
    }
}