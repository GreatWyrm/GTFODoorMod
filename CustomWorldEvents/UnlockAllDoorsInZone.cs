using GameData;
using LevelGeneration;
using UnityEngine;

namespace GTFODoorMod.CustomWorldEvents;

public class UnlockAllDoorsInZone : AbstractWorldEvent
{
    public override System.String Identifier => "UnlockAllDoorsInZone";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            var weakDoors = GetAllWeakDoorsInZone(targetZone);
            foreach (var weakDoor in weakDoors)
            {
                DoorLockTracker.UnlockDoor(weakDoor.MapperDataID);
                foreach (var doorButton in weakDoor.m_buttons)
                {
                    foreach (var sprite in doorButton.gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true)) 
                    {
                        if (sprite.name.Equals(WorldEventsPatcher.SpriteName))
                        {
                            GameObject.Destroy(sprite.gameObject);
                        }
                        if (WorldEventsPatcher.DoorSpriteRenderers.Contains(sprite.name))
                        {
                            sprite.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        else
        {
            eventLogger.LogError($"{Identifier} event failed to get target zone!");
        }
    }
}