using BepInEx.Logging;
using HarmonyLib;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class DoorPatcher
{
    private static readonly ManualLogSource DoorLogger = new("giginss-doormod-door-patcher");
    
    public DoorPatcher()
    {
        Logger.Sources.Add(DoorLogger);
        
        DoorLogger.LogInfo("Patching door method");
        var originalInteractionEvent = typeof(LG_WeakDoor).GetMethod(nameof(LG_WeakDoor.AttemptOpenCloseInteraction));
        var harmony = new Harmony("com.giginss.doormod");
        harmony.Patch(originalInteractionEvent, prefix: new HarmonyMethod(typeof(DoorPatcher), nameof(OpenCloseInteractionPrefix)));
        DoorLogger.LogInfo("Patching successful!");
    }

    static bool OpenCloseInteractionPrefix(LG_WeakDoor __instance, bool onlyUnlock)
    {
        DoorLogger.LogInfo($"Door interaction with {__instance.MapperDataID}");
        if (DoorLockTracker.IsDoorLocked(__instance.MapperDataID))
        {
            DoorLogger.LogInfo($"Door interaction with {__instance.MapperDataID} was denied from the lock.");
            return false;
        }
        return true;
    }
}