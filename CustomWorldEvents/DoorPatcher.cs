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
        var originalOpenCloseInteractionMethod = typeof(LG_WeakDoor).GetMethod(nameof(LG_WeakDoor.AttemptOpenCloseInteraction));
        var originalInteractionAllowedMethod = typeof(LG_WeakDoor).GetMethod(nameof(LG_WeakDoor.InteractionAllowed));
        var harmony = new Harmony("com.giginss.doormod");
        harmony.Patch(originalOpenCloseInteractionMethod, prefix: new HarmonyMethod(typeof(DoorPatcher), nameof(OpenCloseInteractionPrefix)));
        // Tried to use this to disable the HUD button interaction, but it didn't work
        //harmony.Patch(originalInteractionAllowedMethod, prefix: new HarmonyMethod(typeof(DoorPatcher), nameof(InteractionAllowedPrefix)));
        DoorLogger.LogInfo("Patching successful!");
    }

    static bool OpenCloseInteractionPrefix(LG_WeakDoor __instance, bool onlyUnlock)
    {
        if (DoorLockTracker.IsDoorLocked(__instance.MapperDataID))
        {
            return false;
        }
        return true;
    }
    
    static bool InteractionAllowedPrefix(LG_WeakDoor __instance)
    {
        if (DoorLockTracker.IsDoorLocked(__instance.MapperDataID))
        {
            return false;
        }
        return true;
    }
}