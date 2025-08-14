using GameData;
using Player;
using SNetwork;

namespace GTFODoorMod.CustomWorldEvents;

public class TeleportPocketItems : AbstractWorldEvent
{
    public override string Identifier => "TeleportPocketItems";

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        int source = eventData.Count;
        int destination = (int)eventData.EnemyID;

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

        var doubleList = sourceBackpack.PocketItemsGroups;
        
        // Copy over and clear
        for (int i = 0; i < doubleList.Count; i++)
        {
            Il2CppSystem.Collections.Generic.List<PocketItem> pocketItemList = doubleList._items[i];
            for (int j = 0; j < pocketItemList.Count; j++)
            {
                // Retrieve pocket item id
                if (pocketItemList.Count > 0)
                {
                    PocketItem pocketItem = pocketItemList._items[0];
                    // Ensure list exists in destination
                    if (!destinationBackpack.ItemIDToPocketItemGroup.ContainsKey(pocketItem.itemID))
                    {
                        destinationBackpack.ItemIDToPocketItemGroup.Add(pocketItem.itemID, new Il2CppSystem.Collections.Generic.List<PocketItem>());
                        destinationBackpack.PocketItemsGroups.Add(destinationBackpack.ItemIDToPocketItemGroup[pocketItem.itemID]);
                    }
                    destinationBackpack.ItemIDToPocketItemGroup[pocketItem.itemID].Add(pocketItem);
                }
            }
            pocketItemList.Clear();
        }
        doubleList.Clear();
        sourceBackpack.ItemIDToPocketItemGroup.Clear();
        eventLogger.LogInfo($"Transferred pocket items from {source} to {destination}.");
        
        // Update GUI
        PlayerBackpackManager.UpdatePocketItemGUI();
    }
}