using System;
using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class UnlockAllDoorsInZone : AbstractWorldEvent
{
    public override String Identifier => "UnlockAllDoorsInZone";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            var weakDoors = GetAllWeakDoorsInZone(targetZone);
            foreach (var weakDoor in weakDoors)
            {
                DoorLockTracker.UnlockDoor(weakDoor.MapperDataID);
            }
        }
        else
        {
            eventLogger.LogError("UnlockAllDoorsInZone event failed to get target zone!");
        }
    }
}