using GameData;
using GTFO.API;

namespace GTFODoorMod.CustomWorldEvents;

public class DisableFallDamage : AbstractWorldEvent
{
    public override string Identifier => "DisableFallDamage";
    private bool NeedsToResetFallValues = false;
    
    public DisableFallDamage()
    {
        LevelAPI.OnLevelCleanup += ResetFallValues;
    }

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        PlayerDataBlock pDataBlock = GameDataBlockBase<PlayerDataBlock>.GetBlock(1U);
        pDataBlock.fallDamageMinHeight = 999f;
        pDataBlock.fallDamageMaxHeight = 1000f;
        NeedsToResetFallValues = true;
    }

    private void ResetFallValues()
    {
        if (!NeedsToResetFallValues) return;
        
        PlayerDataBlock pDataBlock = GameDataBlockBase<PlayerDataBlock>.GetBlock(1U);
        pDataBlock.fallDamageMinHeight = 4f;
        pDataBlock.fallDamageMaxHeight = 20f;
        NeedsToResetFallValues = false;
    }
}