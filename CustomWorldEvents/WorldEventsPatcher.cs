using System.Collections;
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

    private static readonly int eWardenObjectiveEventTypeOffset = 50;
    private int currentCount = 0;

    public WorldEventsPatcher(Harmony harmony)
    {
        BepInEx.Logging.Logger.Sources.Add(EventsLogger);
        
        EventsLogger.LogDebug("Creating door events");        
        LockAllDoorsInZone lockDoorsEvent = new();
        UnlockAllDoorsInZone unlockDoorsEvent = new();
        OpenAllWeakDoorsInZone openAllDoorsEvent = new();

        EventsLogger.LogDebug("Registering events");
        AddToCustomWorldEvents(lockDoorsEvent);
        AddToCustomWorldEvents(unlockDoorsEvent);
        AddToCustomWorldEvents(openAllDoorsEvent);
        
        EventsLogger.LogDebug("Injecting events into enum...");
        foreach (var pair in _customWorldEvents)
        {
            _enumMapping[pair.Value.Identifier] = pair.Key;
        }
        EnumInjector.InjectEnumValues<eWardenObjectiveEventType>(_enumMapping);
        
        EventsLogger.LogDebug("Injection complete, patching...");

        var originalExecuteEvent = typeof(WorldEventManager).GetMethod(nameof(WorldEventManager.ExecuteEvent));
        harmony.Patch(originalExecuteEvent, prefix: new HarmonyMethod(typeof(WorldEventsPatcher), nameof(ExecuteEventPrefix)));
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

    private static IEnumerator ExecuteEventCoroutine(AbstractWorldEvent abstractWorldEvent, WardenObjectiveEventData eData, float delay)
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