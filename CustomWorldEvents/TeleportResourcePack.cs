using GameData;
using Player;
using SNetwork;

namespace GTFODoorMod.CustomWorldEvents;

public class TeleportResourcePack : AbstractWorldEvent
{
    public override string Identifier => "TeleportResourcePack";

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        int source = eventData.Count;
        int destination = (int)eventData.EnemyID;

        if (!SNet.IsMaster)
            return;

        var playersInLevel = PlayerManager.PlayerAgentsInLevel;
        if (source >= playersInLevel.Count || destination >= playersInLevel.Count)
        {
            eventLogger.LogWarning($"Either {source} or {destination} are out of range when trying to teleport pocket items. Number of player agents is {playersInLevel.Count}.");
            return;
        }
        var sourceAgent = playersInLevel[source];
        var destinationAgent = playersInLevel[destination];

        bool gotSourceBackpack = PlayerBackpackManager.TryGetBackpack(sourceAgent.Owner, out var sourceBackpack);
        bool gotDestBackpack =
            PlayerBackpackManager.TryGetBackpack(destinationAgent.Owner, out var destinationBackpack);
        if (!gotSourceBackpack || !gotDestBackpack)
        {
            eventLogger.LogWarning(
                $"Failed to get source and/or destination backpack for indices {source} and {destination}!");
            return;
        }

        var gotSourceResourcePack =
            sourceBackpack.TryGetBackpackItem(InventorySlot.ResourcePack, out var sourceResourcePackItem);
        var gotDestResourcePack = destinationBackpack.TryGetBackpackItem(InventorySlot.ResourcePack,
            out var destinationResourcePackItem);

        if (gotSourceResourcePack)
        {
            var custom = new pItemData_Custom();
            custom.ammo = sourceBackpack.AmmoStorage.ResourcePackAmmo.AmmoInPack;
            eventLogger.LogInfo($"Set source custom.ammo to {custom.ammo}.");
            sourceResourcePackItem.Instance.SetCustomData(custom, false);
        }

        if (gotDestResourcePack)
        {
            var custom = new pItemData_Custom();
            custom.ammo = destinationBackpack.AmmoStorage.ResourcePackAmmo.AmmoInPack;
            eventLogger.LogInfo($"Set destination custom.ammo to {custom.ammo}.");
            destinationResourcePackItem.Instance.SetCustomData(custom, false);
        }

        if (gotDestResourcePack && gotSourceResourcePack)
        {
            // Both source and destination are holding an item, swap
            PlayerBackpackManager.MasterRemoveItem(sourceResourcePackItem.Instance, sourceBackpack);
            PlayerBackpackManager.MasterRemoveItem(destinationResourcePackItem.Instance, destinationBackpack);
            PlayerBackpackManager.MasterAddItem(destinationResourcePackItem.Instance, sourceBackpack);
            PlayerBackpackManager.MasterAddItem(sourceResourcePackItem.Instance, destinationBackpack);
            eventLogger.LogInfo($"Swapped resource packs between indices {source} and {destination}.");
        }
        else if (!gotDestResourcePack)
        {
            // Destination is not holding anything, allow source to send to destination
            PlayerBackpackManager.MasterRemoveItem(sourceResourcePackItem.Instance, sourceBackpack);
            PlayerBackpackManager.MasterAddItem(sourceResourcePackItem.Instance, destinationBackpack);
            eventLogger.LogInfo($"Sent resource pack from {source} to {destination}.");
        }
        else
        {
            // Source does not have a resource pack to swap, do nothing
            eventLogger.LogInfo($"Failed to teleport resource packs from {source} to {destination}! ({gotSourceResourcePack}, {gotDestResourcePack})");
        }
    }
}