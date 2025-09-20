namespace BetterScannerRoom;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using System.Globalization;
using UnityEngine;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
[BepInIncompatibility("com.ahk1221.smlhelper")]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    private static string GetSaveFilePath => Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "FoundScanTypes.json");
    internal static new ManualLogSource Logger;
    private MapInputHandler inputHandler = new();

    private void Awake()
    {
        Logger = base.Logger;
        BSRSettings.Instance.Load();
        SaveUtils.RegisterOnSaveEvent(SaveCache);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

        Logger.LogInfo($"Loaded {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}");
    }

    public void Update()
    {
        if (inputHandler.mapRoom == null)
        {
            foreach (var map in BSRSettings.MapRooms)
            {
                if (map.playerDistanceTracker.distanceToPlayer > 6f)
                    continue;

                inputHandler.mapRoom = map;
                break;
            }
        }

        if (inputHandler.mapRoom == null)
            return;

        if (inputHandler.mapRoom.playerDistanceTracker.distanceToPlayer > 6f)
        {
            inputHandler.canHandleInput = false;
            inputHandler.mapRoom = null;
            return;
        }

        if (Inventory.main != null && Inventory.main.GetHeldObject() == null)
        {
            // Get current input device across SN/BZ API differences
#if BELOWZERO
            var device = GameInput.GetPrimaryDevice();
#else
            var device = GameInput.PrimaryDevice;
#endif
            var reloadBind = GameInput.GetBinding(device, GameInput.Button.Reload, GameInput.BindingSet.Primary);
            var leftBind = GameInput.GetBinding(device, GameInput.Button.LeftHand, GameInput.BindingSet.Primary);
            var rightBind = GameInput.GetBinding(device, GameInput.Button.RightHand, GameInput.BindingSet.Primary);
            ProcessMSG($"Press {reloadBind} to switch to Holographic Controls.", !inputHandler.canHandleInput);
            ProcessMSG($"Press {reloadBind} to exit Holographic Controls.", inputHandler.canHandleInput);
            ProcessMSG($"Hold {leftBind} to raise/lower map by looking up/down.", inputHandler.canHandleInput);
            ProcessMSG($"Hold {rightBind} to rotate map by looking left/right.", inputHandler.canHandleInput);
            if (GameInput.GetButtonDown(GameInput.Button.Reload))
            {
                inputHandler.canHandleInput = !inputHandler.canHandleInput;
                if (inputHandler.canHandleInput)
                {
                    InputHandlerStack.main.Push(inputHandler);
                }
            }
        }
    }

    internal static void ProcessMSG(string msg, bool active)
    {
        var message = ErrorMessage.main.GetExistingMessage(msg);
        if (active)
        {
            if (message != null)
            {
                message.messageText = msg;
                message.entry.text = msg;
                if (message.timeEnd <= Time.time + 1f)
                    message.timeEnd += Time.deltaTime;
            }
            else
            {
                ErrorMessage.AddMessage(msg);
            }
        }
        else if (message != null && message.timeEnd > Time.time)
        {
            message.timeEnd = Time.time;
        }
    }

    public static void SaveCache()
    {
        Dictionary<TechType, Dictionary<string, ResourceTrackerDatabase.ResourceInfo>> TrackedResources = ResourceTrackerDatabase.resources;

        Dictionary<string, Dictionary<string, ResourceTrackerDatabase.ResourceInfo>> stringifiedTrackedResources = new();

        foreach (var kvp in TrackedResources)
        {
            stringifiedTrackedResources[kvp.Key.AsString()] = kvp.Value;
        }


        var writer = new StreamWriter(GetSaveFilePath);
        try
        {
            writer.Write(
                JsonConvert.SerializeObject(
                    stringifiedTrackedResources, new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Culture = CultureInfo.InvariantCulture,
                        NullValueHandling = NullValueHandling.Ignore

                    })
            );
            writer.Flush();
            writer.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e}");
            writer.Close();
        }

    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPostfix]
    public static void LoadCache()
    {
        if (!File.Exists(GetSaveFilePath))
            return;
        var reader = new StreamReader(GetSaveFilePath);
        try
        {
            Dictionary<string, Dictionary<string, ResourceTrackerDatabase.ResourceInfo>> stringifiedTrackedResources = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ResourceTrackerDatabase.ResourceInfo>>>(
                reader.ReadToEnd(),
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                }
            );
            reader.Close();

            foreach (var pair in stringifiedTrackedResources)
            {
                if (Enum.TryParse(pair.Key, out TechType techType))
                    ResourceTrackerDatabase.resources[techType] = pair.Value;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            reader.Close();
        }
    }
}
