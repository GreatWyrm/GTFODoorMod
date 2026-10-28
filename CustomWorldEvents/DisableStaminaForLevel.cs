using GameData;
using GTFO.API;
using Player;

namespace GTFODoorMod.CustomWorldEvents;

public class DisableStaminaForLevel : AbstractWorldEvent
{
    
    private bool NeedsToResetStaminaValues = false;

    public DisableStaminaForLevel()
    {
        LevelAPI.OnLevelCleanup += ResetStaminaValues;
    }
    
    public override System.String Identifier => "DisableStaminaForLevel";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
        {
            playerAgent.PlayerData.StaminaEnableAffectMeleeSpeed = false;
            playerAgent.PlayerData.StaminaEnableAffectMoveSpeed = false;
            playerAgent.PlayerData.StaminaEnableAffectStrafeSpeed = false;
            playerAgent.PlayerData.breathingEnabled = false;
            NeedsToResetStaminaValues = true;
        }
    }

    public void ResetStaminaValues()
    {
        if (NeedsToResetStaminaValues && PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent agent))
        {
            agent.PlayerData.StaminaEnableAffectMeleeSpeed = true;
            agent.PlayerData.StaminaEnableAffectMoveSpeed = true;
            agent.PlayerData.StaminaEnableAffectStrafeSpeed = true;
            agent.PlayerData.breathingEnabled = true;
            NeedsToResetStaminaValues = false;
        }
    }
}