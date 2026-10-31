using GameData;
using Player;
using SNetwork;

namespace GTFODoorMod.CustomWorldEvents;

public class RefillAmmo : AbstractWorldEvent
{
    public override string Identifier => "RefillAmmo";

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        if (!SNet.IsMaster) return;

        foreach (var player in PlayerManager.PlayerAgentsInLevel)
        {
            if (PlayerBackpackManager.TryGetBackpack(player.Owner, out PlayerBackpack backpack))
            {
                var ammoStorage = backpack.AmmoStorage;
                ammoStorage.StandardAmmo.AddAmmo(ammoStorage.StandardAmmo.AmmoMaxCap);
                ammoStorage.SpecialAmmo.AddAmmo(ammoStorage.SpecialAmmo.AmmoMaxCap);
                ammoStorage.ClassAmmo.AddAmmo(ammoStorage.ClassAmmo.AmmoMaxCap);
            }
            else
            {
                eventLogger.LogWarning($"Failed to get backpack for {player.Owner.NickName}!");
            }
        }
    }
}