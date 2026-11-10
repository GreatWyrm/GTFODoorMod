using GameData;
using GTFO.API;

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
        PlayerDataBlock pDataBlock = GameDataBlockBase<PlayerDataBlock>.GetBlock(1U);
        pDataBlock.StaminaEnableAffectMeleeSpeed = false;
        pDataBlock.StaminaEnableAffectMoveSpeed = false;
        pDataBlock.StaminaEnableAffectStrafeSpeed = false;
        pDataBlock.breathingEnabled = false;
        NeedsToResetStaminaValues = true;
    }

    public void ResetStaminaValues()
    {
        if (!NeedsToResetStaminaValues) return;
        
        PlayerDataBlock pDataBlock = GameDataBlockBase<PlayerDataBlock>.GetBlock(1U);
        pDataBlock.StaminaEnableAffectMeleeSpeed = true;
        pDataBlock.StaminaEnableAffectMoveSpeed = true;
        pDataBlock.StaminaEnableAffectStrafeSpeed = true;
        pDataBlock.breathingEnabled = true;
        NeedsToResetStaminaValues = false;
    }
}