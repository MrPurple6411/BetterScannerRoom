namespace BetterScannerRoom;

using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using System;
using System.Collections.Generic;

[Menu(MyPluginInfo.PLUGIN_NAME, LoadOn = MenuAttribute.LoadEvents.MenuOpened)]
internal class BSRSettings: ConfigFile
{
    internal static HashSet<MapRoomFunctionality> MapRooms = new();

    [Slider("Default Speed", 1f, 60f, Order = 0), OnChange(nameof(ApplyOptions))]
    public float scannerSpeedDefaultInterval = 14f;

    [Slider("Speed Per Module", 1f, 60f, Step = 1f, Order = 1), OnChange(nameof(ApplyOptions))]
    public float scannerSpeedIntervalPerModule = 3f;

    [Slider("Default Range", 0f, 1500f, DefaultValue = 300f, Step = 50f, Order = 2), OnChange(nameof(ApplyOptions))]
    public float scannerDefaultRange = 300f;

    [Slider("Range Per Module", 0f, 1500f, DefaultValue = 50f, Step = 25f, Order = 3), OnChange(nameof(ApplyOptions))]
    public float scannerUpgradeAddedRange = 50f;

    [Slider("Max Range", 0f, 3000f, DefaultValue = 1500f, Step = 50f, Order = 4), OnChange(nameof(ApplyOptions))]
    public float scannerMaxRange = 1500f;

    [Slider("Blip Range", 0f, 3000f, DefaultValue = 500f, Step = 50f, Order = 5)]
    public float scannerBlipRange = 500f;

    [Slider("Camera Range", 0f, 3000f, DefaultValue = 1000f, Step = 100f, Order = 6), OnChange(nameof(ApplyOptions))]
    public float scannerCameraRange = 500f;

    // toggle for the scannerMapPulse
    [Toggle("Map Pulse", Order = 7), OnChange(nameof(TogglePulse))]
    public bool mapPulseEnabled = true;

    private static void TogglePulse()
    {
        foreach (var map in MapRooms)
        {
            map.prevScanInterval = -1f;
        }
    }

    private static void ApplyOptions()
    {
        foreach (var map in MapRooms)
        {
            map.containerIsDirty = true;
            map.miniWorld.ClearAllChunks();
        }
    }

    public static BSRSettings Instance { get; } = OptionsPanelHandler.RegisterModOptions<BSRSettings>();

    public static float ScannerCameraRange() => Instance.scannerCameraRange;
    public static float ScannerBlipRange() => Instance.scannerBlipRange;
    public static float ScannerDefaultRange() => Instance.scannerDefaultRange;
    public static float ScannerMaxRange() => Instance.scannerMaxRange;
    public static float ScannerUpgradeAddedRange() => Instance.scannerUpgradeAddedRange;
    public static float ScannerSpeedDefaultInterval() => Instance.scannerSpeedDefaultInterval;
    public static float ScannerSpeedIntervalPerModule() => Instance.scannerSpeedIntervalPerModule;
    public static float GetExtendedCameraRange() => ScannerCameraRange() * 2.05f;
    public static bool MapPulseEnabled => Instance.mapPulseEnabled;
}