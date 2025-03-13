
using GameData;
using Player;

namespace GTFODoorMod.CustomWorldEvents;

public class ForgetLastKnownMainDimPosition : AbstractWorldEvent
{
    public override System.String Identifier => "ForgetLastKnownMainDimPosition";
    
    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        var agents = PlayerManager.PlayerAgentsInLevel._items;
        for (int i = 0; i < agents.Count; i++)
        {
            PlayerAgent playerAgent = agents[i];
            if (playerAgent != null)
            {
                playerAgent.HasLastKnownMainDimensionPosition = false;
                eventLogger.LogInfo($"{playerAgent.PlayerName} has forgotten their last known dim position!");
            }
        }
    }
}