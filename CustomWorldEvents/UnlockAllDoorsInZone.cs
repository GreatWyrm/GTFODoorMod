using System;
using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class UnlockAllDoorsInZone : AbstractWorldEvent
{
    public override String Identifier => "UnlockAllDoorsInZone";

    public override void onEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            foreach (var courseNode in targetZone.m_courseNodes)
            {
                foreach (var coursePortal in courseNode.m_portals)
                {
                    if (coursePortal.m_door?.DoorType == eLG_DoorType.Weak)
                    {
                        LG_WeakDoor weakDoor = coursePortal.m_door.TryCast<LG_WeakDoor>();
                        if (weakDoor != null)
                        {
                            eventLogger.LogInfo($"Unlocking weak door with id: {weakDoor.MapperDataID}");
                            DoorLockTracker.UnlockDoor(weakDoor.MapperDataID);
                        }
                    }
                }
            }
        }
        else
        {
            eventLogger.LogInfo("UnlockAllDoorsInZone event failed to get target zone!");
        }
    }
}