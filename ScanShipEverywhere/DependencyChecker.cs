using System.Linq;
using BepInEx.Bootstrap;

namespace ScanShipEverywhere;

public static class DependencyChecker {
    public static bool IsBetterItemScanModInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Equals("PopleZoo.BetterItemScan"));

    public static bool IsGeneralImprovementsInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Equals("ShaosilGaming.GeneralImprovements"));
    
    public static bool IsScanTweaksModInstalled() =>
        Chainloader.PluginInfos.Values.Any(metadata => metadata.Metadata.GUID.Equals("Saradora.ScanTweaks"));
}