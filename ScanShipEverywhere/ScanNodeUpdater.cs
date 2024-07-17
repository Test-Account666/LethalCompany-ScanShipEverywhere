using GameNetcodeStuff;
using UnityEngine;

namespace ScanShipEverywhere;

public class ScanNodeUpdater : MonoBehaviour {
    private ScanNodeProperties? _scanNodeProperties;
    private GameObject _parent = null!;
    private Component? _component;
    private float _nextUpdate = .5F;

    internal void SetScanNodeProperties(ScanNodeProperties scanNodeProperties) {
        _scanNodeProperties = scanNodeProperties;
        _parent = _scanNodeProperties.gameObject.transform.parent.gameObject;
    }

    internal bool IsValid() {
        if (_scanNodeProperties is null) return false;

        _component = _parent.GetComponent<Terminal>();
        if (_component is not null) return true;

        _component = _parent.GetComponent<ItemDropship>();
        if (_component is not null) return true;

        _component = _parent.GetComponent<EntranceTeleport>();
        return _component is not null || _scanNodeProperties.headerText.ToLower().Contains("entrance");
    }

    private void Update() {
        if (_scanNodeProperties is null) return;
        _nextUpdate -= Time.deltaTime;

        if (_nextUpdate > 0) return;

        _nextUpdate = .5F;

        var localPlayer = StartOfRound.Instance.localPlayerController;

        // Increase/Decrease the maxRange for specific scan nodes

        HandleDropShip(localPlayer, _scanNodeProperties);

        HandleEntrance(localPlayer, _scanNodeProperties);

        HandleSpaceShip(localPlayer, _scanNodeProperties);
    }

    private void HandleEntrance(PlayerControllerB playerControllerB, ScanNodeProperties scanNodeProperties) {
        var maxRange = ScanShipEverywhere.configManager.GetMaxEntranceDistance();

        // The main entrance scan node is not bound to the actual entrance in vanilla maps... For some reason ¯\_(ツ)_/¯
        if (scanNodeProperties.headerText.ToLower().Contains("entrance")) {
            if (playerControllerB.isInsideFactory) maxRange = 2;

            UpdateScanNode(scanNodeProperties, false, 1, maxRange);
            return;
        }

        // In case of custom maps that might add the scan node to the actual thing

        if (_component is not EntranceTeleport entranceTeleport) return;

        maxRange = ScanShipEverywhere.configManager.GetMaxEntranceDistance();

        switch (entranceTeleport.isEntranceToBuilding) {
            case true when playerControllerB.isInsideFactory:
            case false when !playerControllerB.isInsideFactory:
                maxRange = 2;
                break;
        }

        UpdateScanNode(scanNodeProperties, false, 1, maxRange);
    }

    private void HandleDropShip(PlayerControllerB playerControllerB, ScanNodeProperties scanNodeProperties) {
        if (_component is not ItemDropship itemDropShip) return;

        var maxRange = itemDropShip.deliveringOrder? ScanShipEverywhere.configManager.GetMaxDropShipDistance() : 2;

        if (playerControllerB.isInsideFactory) maxRange = 2;

        UpdateScanNode(scanNodeProperties, false, 1, maxRange);
    }

    private void HandleSpaceShip(PlayerControllerB playerControllerB, ScanNodeProperties scanNodeProperties) {
        if (_component is not Terminal terminal) return;

        var maxRange = ScanShipEverywhere.configManager.GetMaxShipDistance();

        if (playerControllerB.isInsideFactory || playerControllerB.isInHangarShipRoom) maxRange = 2;

        UpdateScanNode(scanNodeProperties, false, 1, maxRange);
    }

    private static void UpdateScanNode(ScanNodeProperties scanNodeProperties, bool requiresLineOfSight, int minRange, int maxRange) {
        scanNodeProperties.requiresLineOfSight = requiresLineOfSight;

        scanNodeProperties.minRange = minRange;
        scanNodeProperties.maxRange = maxRange;
    }
}