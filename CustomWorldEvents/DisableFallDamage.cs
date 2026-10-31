using GameData;
using GTFO.API;
using Player;

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
        if (PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
        {
            playerAgent.PlayerData.fallDamageMinHeight = 999f;
            playerAgent.PlayerData.fallDamageMinHeight = 1000f;
            NeedsToResetFallValues = true;
        }
    }

    private void ResetFallValues()
    {
        if (this.NeedsToResetFallValues && PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
        {
            playerAgent.PlayerData.fallDamageMinHeight = 4f;
            playerAgent.PlayerData.fallDamageMinHeight = 20f;
            this.NeedsToResetFallValues = false;
        }
    }
}