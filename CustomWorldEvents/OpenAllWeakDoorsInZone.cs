using System;
using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class OpenAllWeakDoorsInZone : AbstractWorldEvent
{
    public override String Identifier => "OpenAllWeakDoorsInZone";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            var weakDoors = GetAllWeakDoorsInZone(targetZone);
            foreach (var weakDoor in weakDoors)
            {
                weakDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Open);
            }
        }
        else
        {
            eventLogger.LogError("LockAllDoorsInZone event failed to get target zone!");
        }
    }
}