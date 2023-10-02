using System;
using System.Collections.Generic;
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

    public abstract void OnEventTrigger(ref WardenObjectiveEventData eventData);
    
    protected static bool TryGetZone(WardenObjectiveEventData eventData, out LG_Zone zone)
    {
        if (!Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventData.DimensionIndex, eventData.Layer, eventData.LocalIndex, out zone))
        {
            zone = null;
            return false;
        }
        return true;
    }

    protected static HashSet<LG_WeakDoor> GetAllWeakDoorsInZone(LG_Zone zone)
    {
        HashSet<LG_WeakDoor> set = new();
        foreach (var courseNode in zone.m_courseNodes)
        {
            foreach (var coursePortal in courseNode.m_portals)
            {
                if (coursePortal.m_door?.DoorType != eLG_DoorType.Weak) continue;
                
                var weakDoor = coursePortal.m_door.TryCast<LG_WeakDoor>();
                if (weakDoor != null)
                {
                    set.Add(weakDoor);
                }
            }
        }

        return set;
    }
}