using GameData;

namespace GTFODoorMod.CustomWorldEvents;

public class ChangeObjectiveHeaderText : AbstractWorldEvent
{
    public override string Identifier => "ChangeObjectiveHeaderText";
    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        GuiManager.PlayerLayer.m_wardenObjective.m_header.text = eventData.WorldEventObjectFilter;
    }
}