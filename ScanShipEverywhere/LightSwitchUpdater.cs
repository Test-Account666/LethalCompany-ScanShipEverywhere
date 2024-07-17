using UnityEngine;

namespace ScanShipEverywhere;

public class LightSwitchUpdater : MonoBehaviour {
    private ScanNodeProperties? _scanNodeProperties;

    private void Awake() => _scanNodeProperties ??= GetComponentInChildren<ScanNodeProperties>();


    private void LateUpdate() {
        if (_scanNodeProperties is null) return;

        var localPlayer = StartOfRound.Instance?.localPlayerController ?? null;

        if (localPlayer is null) return;

        if (_scanNodeProperties.enabled && !ScanShipEverywhere.configManager.AddLightSwitchScanNode()) {
            _scanNodeProperties.enabled = false;
            return;
        }

        if (_scanNodeProperties.enabled == localPlayer.isInHangarShipRoom) return;

        _scanNodeProperties.enabled = localPlayer.isInHangarShipRoom;
    }
}