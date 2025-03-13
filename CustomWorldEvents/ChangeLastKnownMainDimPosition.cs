
using GameData;
using LevelGeneration;
using Player;

namespace GTFODoorMod.CustomWorldEvents;

public class ChangeLastKnownMainDimPosition : AbstractWorldEvent
{
    public override System.String Identifier => "ForgetLastKnownMainDimPosition";
    
    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        var agents = PlayerManager.PlayerAgentsInLevel._items;
        TryGetZone(eData, out LG_Zone targetZone);
        for (int i = 0; i < agents.Count; i++)
        {
            PlayerAgent playerAgent = agents[i];
            if (playerAgent != null)
            {
                if (eData.Enabled)
                {
                    if (targetZone.m_courseNodes._items.Count > 0)
                    {
                        var courseNode = targetZone.m_courseNodes._items[0];
                        playerAgent.HasLastKnownMainDimensionPosition = true;
                        playerAgent.LastKnownMainDimensionPosition = courseNode.GetRandomPositionInside();
                        playerAgent.LastKnownMainDimensionCourseNode = courseNode;
                        eventLogger.LogInfo($"{playerAgent.PlayerName} has had their last known dimension position changed to a random position inside courseNode 0 for {targetZone.name}.");
                    }
                    else
                    {
                        eventLogger.LogInfo($"No course nodes found for zone {targetZone.name}! Resetting last known dimension position instead.");
                        playerAgent.HasLastKnownMainDimensionPosition = false;
                    }
                }
                else
                {
                    playerAgent.HasLastKnownMainDimensionPosition = false;
                    eventLogger.LogInfo($"{playerAgent.PlayerName} has forgotten their last known dim position!");
                }

            }
        }
    }
}