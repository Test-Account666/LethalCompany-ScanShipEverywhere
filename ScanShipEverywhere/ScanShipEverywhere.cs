using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ScanShipEverywhere.Patches;

namespace ScanShipEverywhere;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("PopleZoo.BetterItemScan", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("Saradora.ScanTweaks", BepInDependency.DependencyFlags.SoftDependency)]
public class ScanShipEverywhere : BaseUnityPlugin {
    internal static ConfigManager configManager = null!;
    public static ScanShipEverywhere Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        Logger.LogInfo("Scan Ship Everywhere has started scanning!");

        configManager = new();

        configManager.Initialize(Config);

        Patch();

        if (DependencyChecker.IsBetterItemScanModInstalled()) {
            Logger.LogInfo("Found BetterItemScan... Doing some extra work for you :)");
            BetterItemScanSupport.Initialize();
        }

        if (DependencyChecker.IsScanTweaksModInstalled()) {
            Logger.LogInfo("Found ScanTweaks... Doing some extra work for you :)");
            Harmony?.PatchAll(typeof(ScanTweaksPingScanPatch));
        }

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch() {
        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll(typeof(HUDManagerPatch));
        Harmony.PatchAll(typeof(RoundManagerPatch));
        Harmony.PatchAll(typeof(ItemDropShipPatch));
        Harmony.PatchAll(typeof(LightSwitchPatch));

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch() {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }
}