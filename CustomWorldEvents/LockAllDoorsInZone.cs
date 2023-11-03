using GameData;
using LevelGeneration;
using UnityEngine;

namespace GTFODoorMod.CustomWorldEvents;

public class LockAllDoorsInZone : AbstractWorldEvent
{
    public override System.String Identifier => "LockAllDoorsInZone";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out LG_Zone targetZone))
        {
            var weakDoors = GetAllWeakDoorsInZone(targetZone);
            Texture2D tex = WorldEventsPatcher.GetRedXTexture();
            Sprite newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            foreach (var weakDoor in weakDoors)
            {
                DoorLockTracker.LockDoor(weakDoor.MapperDataID);
                foreach (var doorButton in weakDoor.m_buttons)
                {
                    foreach (var sprite in doorButton.gameObject.GetComponentsInChildren<SpriteRenderer>()) 
                    {
                        if (sprite.name.Equals("DoorFrame"))
                        {
                            SpriteRenderer clone = GameObject.Instantiate(sprite, sprite.transform.parent.gameObject.transform);
                            clone.name = WorldEventsPatcher.SpriteName;
                            clone.sprite = newSprite;
                            clone.enabled = true;
                        }
                        if (WorldEventsPatcher.DoorSpriteRenderers.Contains(sprite.name))
                        {
                            sprite.enabled = false;
                            sprite.forceRenderingOff = true;
                            //eventLogger.LogInfo($"Successfully disabled {sprite.name}, Force Rendering Off: {sprite.forceRenderingOff}");
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