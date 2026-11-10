using GameData;
using GTFO.API;

namespace GTFODoorMod.CustomWorldEvents;

public class SetFriendlyFireMulti : AbstractWorldEvent
{
    public override string Identifier => "SetFriendlyFireMulti";

    private bool NeedToResetValue = false;
    private readonly float DefaultFriendlyFireMulti = 0.5f;

    public SetFriendlyFireMulti()
    {
        LevelAPI.OnLevelCleanup += ResetValue;
    }

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        PlayerDataBlock pDataBlock = GameDataBlockBase<PlayerDataBlock>.GetBlock(1U);
        pDataBlock.friendlyFireMulti = eventData.Duration;
        NeedToResetValue = true;
    }

    private void ResetValue()
    {
        PlayerDataBlock pDataBlock = GameDataBlockBase<PlayerDataBlock>.GetBlock(1U);
        pDataBlock.friendlyFireMulti = DefaultFriendlyFireMulti;
        NeedToResetValue = true;
    }
}