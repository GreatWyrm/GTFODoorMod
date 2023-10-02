using System;
using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class LockAllDoorsInZone : AbstractWorldEvent
{
    public override String Identifier => "LockAllDoorsInZone";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            var weakDoors = GetAllWeakDoorsInZone(targetZone);
            foreach (var weakDoor in weakDoors)
            {
                DoorLockTracker.LockDoor(weakDoor.MapperDataID);
            }
        }
        else
        {
            eventLogger.LogError("LockAllDoorsInZone event failed to get target zone!");
        }
    }
}