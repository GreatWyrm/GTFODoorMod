using BepInEx.Logging;
using CellMenu;
using GTFO.API;
using HarmonyLib;
using SNetwork;
using UnityEngine;

namespace GTFODoorMod.Temperature;

public class TemperatureManager
{

    private GameObject temperatureTracker;
    public static bool IsTemperatureActive = false;
    private static readonly ManualLogSource Logger = new("giginss.rundownmod.temperature");

    public TemperatureManager(Harmony harmony)
    {
        BepInEx.Logging.Logger.Sources.Add(Logger);
        Logger.LogInfo("Initializing Temperature Manager");
        LevelAPI.OnLevelCleanup += OnLevelCleanup;
        RegisterNetworkEvents();
        RegisterPatches(harmony);
        Logger.LogInfo("Temperature Manager initialized");
    }

    public void ActivateTemperature(float startingValue, float increaseRate)
    {
        if (temperatureTracker == null)
        {
            temperatureTracker = new GameObject("TemperatureTracker");
            temperatureTracker.AddComponent<TemperatureUpdater>();
            temperatureTracker.GetComponent<TemperatureUpdater>().SetupCallbacks(UpdateTemperatureDisplay, SyncCurrentTemperature);
        }
        temperatureTracker.GetComponent<TemperatureUpdater>().StartTemperatureTicking(startingValue, increaseRate);
        GuiManager.PlayerLayer.m_objectiveTimer.SetTimerActive(true, true);
        GuiManager.PlayerLayer.m_objectiveTimer.m_titleText.SetText("Temperature Status");
        IsTemperatureActive = true;
    }

    public void AddTemporaryCooling(float duration, float coolingRate)
    {
        if (temperatureTracker != null)
        {
            temperatureTracker.GetComponent<TemperatureUpdater>().AddTemporaryCooling(duration, coolingRate);
        }
    }

    public void DeactivateTemperature()
    {
        if (temperatureTracker != null)
        {
            temperatureTracker.GetComponent<TemperatureUpdater>().StopTemperatureTicking();
            GuiManager.PlayerLayer.m_objectiveTimer.SetTimerActive(false, true);
        }
        IsTemperatureActive = false;
    }

    public void OnLevelCleanup()
    {
        if (temperatureTracker != null)
        {
            GameObject.Destroy(temperatureTracker);
            temperatureTracker = null;
        }

        IsTemperatureActive = false;
    }

    private void UpdateTemperatureDisplay(float currentTemp, Color textColor, string status)
    {
        if (IsTemperatureActive)
        {
            GuiManager.PlayerLayer.m_objectiveTimer.m_timerText.SetText($"{currentTemp:0.00} °C, Condition: <color=#{ColorUtility.ToHtmlStringRGBA(textColor)}>{status}</color>");
        }
    }
    
    private void SyncCurrentTemperature(float currentTemp)
    {
        if (SNet.IsMaster && IsTemperatureActive)
        {
            NetworkAPI.InvokeEvent(SYNC_TEMP, currentTemp, SNet_ChannelType.GameOrderCritical);
        }
    }
    
    
    public void SetTemperatureBounds(float lower, float upper)
    {
        if (temperatureTracker != null)
        {
            temperatureTracker.GetComponent<TemperatureUpdater>().SetTemperatureBounds(lower, upper);
        }
    }
    
    // NETWORKING

    private readonly string SYNC_TEMP = "SyncTemperature";

    private void RegisterNetworkEvents()
    {
        NetworkAPI.RegisterEvent<float>(SYNC_TEMP, SyncTemperature);
    }

    public void SyncTemperature(ulong id, float temperature)
    {
        if (temperatureTracker != null)
        {
            temperatureTracker.GetComponent<TemperatureUpdater>().SetTemperatureFromSync(temperature);
        }
    }
    
    // PATCHES

    private void RegisterPatches(Harmony harmony)
    {
        var originalSetInfection = typeof(PUI_LocalPlayerStatus).GetMethod(nameof(PUI_LocalPlayerStatus.SetInfectionbar));
        harmony.Patch(originalSetInfection, postfix: new HarmonyMethod(typeof(TemperaturePatches), nameof(TemperaturePatches.SetInfectionBarPostfix)));
        var originalUpdateInfection = typeof(PUI_LocalPlayerStatus).GetMethod(nameof(PUI_LocalPlayerStatus.UpdateInfection));
        harmony.Patch(originalUpdateInfection, postfix: new HarmonyMethod(typeof(TemperaturePatches), nameof(TemperaturePatches.UpdateInfectionPostfix)));
        var originalUpdateInventory = typeof(CM_PageMap).GetMethod(nameof(CM_PageMap.UpdatePlayerInventory));
        harmony.Patch(originalUpdateInventory, postfix: new HarmonyMethod(typeof(TemperaturePatches), nameof(TemperaturePatches.UpdatePlayerInventoryPostfix)));
    }
}