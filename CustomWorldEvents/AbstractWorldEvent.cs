using System;
using System.Collections.Generic;
using System.Linq;
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
    
    protected bool TryGetZone(WardenObjectiveEventData eventData, out LG_Zone zone)
    {
        if (!Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventData.DimensionIndex, eventData.Layer, eventData.LocalIndex, out zone))
        {
            zone = null;
            return false;
        }
        return true;
    }
    
    public bool CheckWorldEventCondition(WardenObjectiveEventData eventData)
    {
        int conditionIndex = eventData.Condition.ConditionIndex;
        if (conditionIndex == -1)
        {
            // No condition set, return true
            return true;
        }
        else
        {
            bool match = eventData.Condition.IsTrue;
            bool currentState = WorldEventManager.GetCondition(conditionIndex);
            return match == currentState;
        }
    }

    protected LG_WeakDoor[] GetAllWeakDoorsInZone(LG_Zone zone)
    {
        // For some reason, if I use a HashSet, weakdoors will still get duplicated
        // Sort by map id
        Dictionary<uint, LG_WeakDoor> table = new();
        foreach (var courseNode in zone.m_courseNodes)
        {
            foreach (var coursePortal in courseNode.m_portals)
            {
                if (coursePortal.m_door?.DoorType != eLG_DoorType.Weak) continue;
                
                var weakDoor = coursePortal.m_door.TryCast<LG_WeakDoor>();
                if (weakDoor != null)
                {
                    table.TryAdd(weakDoor.MapperDataID, weakDoor);
                }
            }
        }
        return table.Values.ToArray();
    }
}