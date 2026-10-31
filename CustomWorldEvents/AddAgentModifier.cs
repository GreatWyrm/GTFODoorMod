using System;
using System.Collections.Generic;
using GameData;
using GTFO.API;
using Player;

namespace GTFODoorMod.CustomWorldEvents;

public class AddAgentModifier : AbstractWorldEvent
{
    public override string Identifier => "AddAgentModifier";

    private readonly List<uint> activeModifiers = new();
    
    public AddAgentModifier()
    {
        LevelAPI.OnLevelCleanup += RemoveAllModifiers;
    }

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {

        if (!Enum.TryParse<AgentModifier>(eventData.WorldEventObjectFilter, out var modifier))
        {
            eventLogger.LogWarning($"Failed to parse agent modifier from {eventData.WorldEventObjectFilter}.");
            return;
        }
        uint modifierKey = AgentModifierManager.AddModifierValue(PlayerManager.GetLocalPlayerAgent(), modifier, eventData.Duration);
        activeModifiers.Add(modifierKey);
    }

    private void RemoveAllModifiers()
    {
        foreach (uint key in activeModifiers)
        {
            AgentModifierManager.ClearModifierChange(key);
        }
        
        activeModifiers.Clear();
    }
}