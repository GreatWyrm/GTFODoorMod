using GameData;
using ModifierAPI;

namespace GTFODoorMod.CustomWorldEvents;

public class AddMovementSpeed : AbstractWorldEvent
{
    public override System.String Identifier => "AddMovementSpeed";

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        MoveSpeedAPI.AddModifier(eventData.Duration, StackLayer.Add, "Maintenance");
    }
}