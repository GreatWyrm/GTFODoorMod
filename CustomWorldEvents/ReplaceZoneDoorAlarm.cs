using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class ReplaceZoneDoorAlarm : AbstractWorldEvent
{
    public override System.String Identifier => "ReplaceZoneDoorAlarm";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        TryGetZone(eData, out LG_Zone zone);

        var securityDoor = zone.m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>();
        if (securityDoor != null)
        {
            ReplicationPatch.OverrideCount++;
            switch (securityDoor.LastStatus)
            {
                // Unsafe states
                case eDoorStatus.ChainedPuzzleActivated:
                case eDoorStatus.Unlocked:
                case eDoorStatus.Open:
                case eDoorStatus.Opening:    
                    eventLogger.LogWarning("Set up chained puzzle on potentially invalid door state.");
                    securityDoor.SetupChainedPuzzleLock(eData.ChainPuzzle);
                    break;
                // Standard states
                case eDoorStatus.Closed:
                case eDoorStatus.Closed_LockedWithKeyItem:
                case eDoorStatus.Closed_LockedWithChainedPuzzle:    
                case eDoorStatus.Closed_LockedWithChainedPuzzle_Alarm:
                case eDoorStatus.Closed_LockedWithPowerGenerator:
                case eDoorStatus.Closed_LockedWithNoKey:
                case eDoorStatus.Closed_LockedWithBulkheadDC:    
                    securityDoor.SetupChainedPuzzleLock(eData.ChainPuzzle);
                    break;
                // Invalid States
                case eDoorStatus.None:
                case eDoorStatus.Closed_BrokenCantOpen:
                case eDoorStatus.Destroyed:
                case eDoorStatus.GluedMax:
                case eDoorStatus.TryOpenStuckBroken:
                case eDoorStatus.TryOpenStuckInGlue:
                    eventLogger.LogError("Tried to setup chained puzzle on invalid door state");
                    break;
                    
            }
            ReplicationPatch.OverrideCount--;
        }
        else
        {
            eventLogger.LogError($"{Identifier} event failed to read a security door!");
        }
    }
}