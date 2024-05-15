namespace ScanShipEverywhere;

public static class GeneralImprovementsSupport {
    public static bool IsFixPersonalScannerEnabled() =>
        GeneralImprovements.Plugin.FixPersonalScanner.Value;
}