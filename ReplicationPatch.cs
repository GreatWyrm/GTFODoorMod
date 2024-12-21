using SNetwork;

namespace GTFODoorMod;

// Copied from https://github.com/AuriRex/GTFO_Gamemodes/blob/master/GTFO_Gamemodes/Patches/Required/ReplicationPatch.cs, appreciate it
internal static class ReplicationPatch
{
    public static bool OverrideSelfManaged => OverrideCount > 0;
    public static uint OverrideCount { get; internal set; } = 0;
    
    public static bool Prefix(SNet_ReplicatorType type, ushort key, ref ushort __result)
    {
        if (!OverrideSelfManaged)
            return true;
        if (type != SNet_ReplicatorType.SelfManaged)
            return true;
        
        __result = (ushort)SNet_Replication.s_highestSlotUsed_SelfManaged;
        SNet_Replication.s_highestSlotUsed_SelfManaged++;
        
        return false;
    }
}