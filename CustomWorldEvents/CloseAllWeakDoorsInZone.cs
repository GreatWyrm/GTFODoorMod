using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class CloseAllWeakDoorsInZone : AbstractWorldEvent
{
    public override System.String Identifier => "CloseAllWeakDoorsInZone";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            var weakDoors = GetAllWeakDoorsInZone(targetZone);
            foreach (var weakDoor in weakDoors)
            {
                weakDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Close);
            }
        }
        else
        {
            eventLogger.LogError($"{Identifier} event failed to get target zone!");
        }
    }
}