using GameData;
using GTFO.API;
using Player;

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
        if (PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
        {
            playerAgent.PlayerData.friendlyFireMulti = eventData.Duration;
            NeedToResetValue = true;
        }
    }

    private void ResetValue()
    {
        if (NeedToResetValue && PlayerManager.TryGetLocalPlayerAgent(out PlayerAgent playerAgent))
        {
            playerAgent.PlayerData.friendlyFireMulti = DefaultFriendlyFireMulti;
            NeedToResetValue = false;
        }
    }
}