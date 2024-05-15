using BepInEx.Configuration;

namespace ScanShipEverywhere;

internal sealed class ConfigManager {
    private ConfigEntry<bool> _addDropShipScanNode = null!;
    private ConfigEntry<string> _dropShipScanNodeHeaderText = null!;
    private ConfigEntry<string> _dropShipScanNodeSubText = null!;
    private ConfigEntry<int> _maxDropShipDistance = null!;
    private ConfigEntry<int> _maxEntranceDistance = null!;
    private ConfigEntry<float> _maxScanDistanceHardLimit = null!;
    private ConfigEntry<int> _maxShipDistance = null!;
    private ConfigEntry<bool> _addLightSwitchScanNode = null!;
    private ConfigEntry<string> _lightSwitchScanNodeHeaderText = null!;
    private ConfigEntry<string> _lightSwitchScanNodeSubText = null!;

    public void Initialize(ConfigFile configFile) {
        _maxDropShipDistance = configFile.Bind("1. Distance", "1. Max Drop Ship Distance", int.MaxValue - 1000,
                                               new ConfigDescription("Defines the maximum scan distance for the drop ship",
                                                                     new AcceptableValueRange<int>(13, int.MaxValue - 1000)));

        _maxEntranceDistance = configFile.Bind("1. Distance", "2. Max Entrance Distance", int.MaxValue - 1000,
                                               new ConfigDescription("Defines the maximum scan distance for the entrance",
                                                                     new AcceptableValueRange<int>(13, int.MaxValue - 1000)));

        _maxShipDistance = configFile.Bind("1. Distance", "3. Max Ship Distance", int.MaxValue - 1000,
                                           new ConfigDescription("Defines the maximum scan distance for the ship",
                                                                 new AcceptableValueRange<int>(13, int.MaxValue - 1000)));

        _addDropShipScanNode = configFile.Bind("2. Drop Ship Scan Node", "Add Drop Ship Scan Node", true,
                                               "If true, will add a scan node to the drop ship (Will not add a node if there already is one)");

        _dropShipScanNodeHeaderText = configFile.Bind("2. Drop Ship Scan Node", "Scan Node Header Text", "Drop Ship",
                                                      "Defines the header text for the drop ship scan node");

        _dropShipScanNodeSubText = configFile.Bind("2. Drop Ship Scan Node", "Scan Node Sub Text", "Get your items :>",
                                                   "Defines the sub text for the drop ship scan node");

        _maxScanDistanceHardLimit = configFile.Bind("3. Extra", "1. Scan Distance Hard Limit", 800F,
                                                    new ConfigDescription(
                                                        "If you encounter lags, try lowering this. This is the hard limit for the scan node distance",
                                                        new AcceptableValueRange<float>(36F, float.MaxValue - 1000F)));

        _addLightSwitchScanNode = configFile.Bind("4. Light Switch Scan Node", "Add Light Switch Scan Node", true,
                                                  "If true, will add a scan node to the light switch (Will not add a node if there already is one)");

        _lightSwitchScanNodeHeaderText = configFile.Bind("4. Light Switch Scan Node", "Scan Node Header Text", "Light Switch",
                                                         "Defines the header text for the light switch scan node");

        _lightSwitchScanNodeSubText = configFile.Bind("4. Light Switch Scan Node", "Scan Node Sub Text", "Toggles the light :3",
                                                      "Defines the sub text for the light switch scan node");
    }

    public int GetMaxDropShipDistance() =>
        _maxDropShipDistance.Value;

    public int GetMaxEntranceDistance() =>
        _maxEntranceDistance.Value;

    public int GetMaxShipDistance() =>
        _maxShipDistance.Value;

    public bool AddDropShipScanNode() =>
        _addDropShipScanNode.Value;

    public string GetDropShipScanNodeHeaderText() =>
        _dropShipScanNodeHeaderText.Value;

    public string GetDropShipScanNodeSubText() =>
        _dropShipScanNodeSubText.Value;

    public float GetMaxScanDistanceHardLimit() =>
        _maxScanDistanceHardLimit.Value;

    public bool AddLightSwitchScanNode() =>
        _addLightSwitchScanNode.Value;

    public string GetLightSwitchScanNodeHeaderText() =>
        _lightSwitchScanNodeHeaderText.Value;

    public string GetLightSwitchScanNodeSubText() =>
        _lightSwitchScanNodeSubText.Value;
}