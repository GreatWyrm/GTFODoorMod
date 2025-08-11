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

        bool gotSourceAgent = PlayerManager.TryGetPlayerAgent(ref source, out var sourceAgent);
        bool gotDestAgent = PlayerManager.TryGetPlayerAgent(ref destination, out var destinationAgent);
        if (!gotSourceAgent || !gotDestAgent)
        {
            eventLogger.LogWarning(
                $"Failed to get source and/or destination agent for indices {source} and {destination}!");
            return;
        }

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