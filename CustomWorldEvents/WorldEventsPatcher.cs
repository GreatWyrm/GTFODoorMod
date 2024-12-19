using System.Collections.Generic;
using BepInEx.Logging;
using GameData;
using GTFO.API.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace GTFODoorMod.CustomWorldEvents;

public class WorldEventsPatcher
{
    private static Dictionary<int, AbstractWorldEvent> _customWorldEvents = new();
    private Dictionary<string, object> _enumMapping = new();
    private static readonly ManualLogSource EventsLogger = new("giginss-doormod-events-patcher");

    private static Texture2D RedXTexture;
    public static readonly List<string> DoorSpriteRenderers = new List<string>() {"DoorFrame", "Door_Blade_1", "Door_Blade_2", "Door_Blade_3", "Door_Blade_4", "Door_Blade_5", "Door_Blade_6", "Door_Blade_7", "Door_Blade_Broken", "DoorEnterArrow_1", "DoorEnterArrow_2", "DoorEnterArrow_3", "DoorExitArrow"};
    public static string SpriteName
    {
        get { return "DoorRedX";  }
    }
    private static readonly int eWardenObjectiveEventTypeOffset = 50;
    private int currentCount = 0;

    public WorldEventsPatcher(Harmony harmony, Texture2D redX)
    {
        BepInEx.Logging.Logger.Sources.Add(EventsLogger);

        RedXTexture = redX;
        // Prevent the texture from unloading if we run a level more than once per session
        UnityEngine.Object.DontDestroyOnLoad(RedXTexture);
        RedXTexture.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        
        EventsLogger.LogDebug("Creating door events");        
        LockAllDoorsInZone lockDoorsEvent = new();
        UnlockAllDoorsInZone unlockDoorsEvent = new();
        OpenAllWeakDoorsInZone openAllDoorsEvent = new();
        CloseAllWeakDoorsInZone closeAllDoorsEvent = new();
        ReplaceZoneDoorAlarm replaceZoneDoorAlarm = new(); 

        EventsLogger.LogDebug("Registering events");
        AddToCustomWorldEvents(lockDoorsEvent);
        AddToCustomWorldEvents(unlockDoorsEvent);
        AddToCustomWorldEvents(openAllDoorsEvent);
        AddToCustomWorldEvents(closeAllDoorsEvent);
        AddToCustomWorldEvents(replaceZoneDoorAlarm);
        
        EventsLogger.LogDebug("Injecting events into enum...");
        foreach (var pair in _customWorldEvents)
        {
            _enumMapping[pair.Value.Identifier] = pair.Key;
        }
        EnumInjector.InjectEnumValues<eWardenObjectiveEventType>(_enumMapping);
        
        EventsLogger.LogDebug("Injection complete, patching...");

        var originalExecuteEvent = typeof(WorldEventManager).GetMethod(nameof(WorldEventManager.ExecuteEvent));
        harmony.Patch(originalExecuteEvent, prefix: new HarmonyMethod(typeof(WorldEventsPatcher), nameof(ExecuteEventPrefix)));

        var originalCleanupEvent = typeof(GS_AfterLevel).GetMethod(nameof(GS_AfterLevel.CleanupAfterExpedition));
        harmony.Patch(originalCleanupEvent, postfix: new HarmonyMethod(typeof(WorldEventsPatcher), nameof(CleanupLevelPostfix)));
        EventsLogger.LogDebug("Patching successful!");
    }

    private void AddToCustomWorldEvents(AbstractWorldEvent worldEvent)
    {
        int index = eWardenObjectiveEventTypeOffset + currentCount;
        currentCount++;
        _customWorldEvents.Add(index, worldEvent);
    }
    
    // Fired when events are processed first
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(WorldEventManager), nameof(WorldEventManager.ExecuteEvent), new Type[] {typeof(WardenObjectiveEventData), typeof(float)})]
    static bool ExecuteEventPrefix(ref WardenObjectiveEventData eData, float currentDuration)
    {
        if (_customWorldEvents.ContainsKey((int)eData.Type))
        {
            if (_customWorldEvents.TryGetValue((int)eData.Type, out AbstractWorldEvent worldEvent))
            {
                EventsLogger.LogDebug($"Triggering {worldEvent.Identifier}");
                float delay = eData.Delay;
                if (delay > 0)
                {
                    CoroutineDispatcher.StartCoroutine(ExecuteEventCoroutine(worldEvent, eData, delay));
                }
                else
                {
                    ExecuteEvent(abstractWorldEvent: worldEvent, eData: eData);
                }
            }
            else
            {
                EventsLogger.LogError($"Key {eData.Type} exists in the custom world events dictionary but failed to retrieve it!?!?!?");
            }
            // Don't pass to original handler
            return false;
        }
        // Otherwise proceed
        return true;
    }

    [HarmonyPostfix]
    static void CleanupLevelPostfix() {
        DoorLockTracker.ClearLockedDoors();
    }

    public static Texture2D GetRedXTexture()
    {
        return RedXTexture;
    }

    private static System.Collections.IEnumerator ExecuteEventCoroutine(AbstractWorldEvent abstractWorldEvent, WardenObjectiveEventData eData, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        ExecuteEvent(abstractWorldEvent: abstractWorldEvent, eData: eData);
    }

    private static void ExecuteEvent(AbstractWorldEvent abstractWorldEvent, WardenObjectiveEventData eData)
    {
        abstractWorldEvent.OnEventTrigger(eventData: ref eData);
        WardenObjectiveManager.DisplayWardenIntel(eData.Layer, eData.WardenIntel);
    }
}