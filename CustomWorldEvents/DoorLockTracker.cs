using System.Collections.Generic;

namespace GTFODoorMod.CustomWorldEvents;

public class DoorLockTracker
{
    private static readonly HashSet<uint> LockedDoors = new();

    public static bool IsDoorLocked(uint doorId)
    {
        return LockedDoors.Contains(doorId);
    }

    public static void LockDoor(uint doorId)
    {
        LockedDoors.Add(doorId);
    }

    public static void UnlockDoor(uint doorId)
    {
        LockedDoors.Remove(doorId);
    }
}