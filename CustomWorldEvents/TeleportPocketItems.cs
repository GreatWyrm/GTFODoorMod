using GameData;
using Player;
using SNetwork;

namespace GTFODoorMod.CustomWorldEvents;

public class TeleportPocketItems : AbstractWorldEvent
{
    public override string Identifier => "TeleportPocketItems";

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        if (!SNet.IsMaster)
        {
            return;
        }
        
        int source = eventData.Count;
        int destination = (int)eventData.EnemyID;

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
                    PocketItem pocketItem = pocketItemList._items[j];
                    
                    var dataWithOwner = new pItemData_WithOwner();
                    dataWithOwner.owningPlayer.SetPlayer(destinationBackpack.Owner);
                    dataWithOwner.data.slot = InventorySlot.InPocket;
                    dataWithOwner.data.itemID_gearCRC = pocketItem.itemID;
                    dataWithOwner.data.replicatorRef = pocketItem.replicatorRef;
                    PlayerBackpackManager.MasterAddItem(dataWithOwner);
                    PlayerBackpackManager.TryMasterRemovePocketItemWithID(pocketItem.itemID, sourceBackpack.Owner);
                }
            }
        }
        eventLogger.LogInfo($"Transferred pocket items from {source} ({sourceBackpack.Owner.NickName}) to {destination} ({destinationBackpack.Owner.NickName}).");
        
        // Update GUI
        PlayerBackpackManager.UpdatePocketItemGUI();
    }
}