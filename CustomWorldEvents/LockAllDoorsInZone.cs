using System;
using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class LockAllDoorsInZone : AbstractWorldEvent
{
    public override String Identifier => "LockAllDoorsInZone";

    public override void onEventTrigger(ref WardenObjectiveEventData eData)
    {
        eventLogger.LogInfo($"Triggering lock doors event, searching for doors in {eData.LocalIndex}");
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            eventLogger.LogInfo($"CourseNode array obtained with size {targetZone.m_courseNodes.Count}");
            foreach (var courseNode in targetZone.m_courseNodes)
            {
                foreach (var coursePortal in courseNode.m_portals)
                {
                    if (coursePortal.m_door?.DoorType == eLG_DoorType.Weak)
                    {
                        LG_WeakDoor weakDoor = coursePortal.m_door.TryCast<LG_WeakDoor>();
                        if (weakDoor != null)
                        {
                            eventLogger.LogInfo($"Locking weak door with id: {weakDoor.MapperDataID}");
                            DoorLockTracker.LockDoor(weakDoor.MapperDataID);
                        }
                    }
                }
            }
        }
        else
        {
            eventLogger.LogInfo("LockAllDoorsInZone event failed to get target zone!");
        }
    }
}