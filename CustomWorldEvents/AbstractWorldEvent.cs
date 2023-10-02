using System;
using BepInEx.Logging;
using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public abstract class AbstractWorldEvent
{
    protected static readonly ManualLogSource eventLogger = new("giginss-doormod-events");

    static AbstractWorldEvent()
    {
        Logger.Sources.Add(eventLogger);
    }
    // The Id to match against, must be a unique string
    public abstract String Identifier { get; }

    public abstract void onEventTrigger(ref WardenObjectiveEventData eventData);
    
    protected static bool TryGetZone(WardenObjectiveEventData eventData, out LG_Zone zone)
    {
        if (!Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventData.DimensionIndex, eventData.Layer, eventData.LocalIndex, out zone))
        {
            zone = null;
            return false;
        }
        return true;
    }
}