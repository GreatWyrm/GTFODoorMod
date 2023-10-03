using BepInEx.Logging;
using HarmonyLib;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class DoorPatcher
{
    private static readonly ManualLogSource DoorLogger = new("giginss-doormod-door-patcher");
    
    public DoorPatcher(Harmony harmony)
    {
        Logger.Sources.Add(DoorLogger);
        
        DoorLogger.LogInfo("Patching door method");
        var originalOpenCloseInteractionMethod = typeof(LG_WeakDoor).GetMethod(nameof(LG_WeakDoor.AttemptOpenCloseInteraction));
        var originalInteractionAllowedMethod = typeof(LG_WeakDoor).GetProperty(nameof(LG_WeakDoor.InteractionAllowed))?.GetGetMethod();
        harmony.Patch(originalOpenCloseInteractionMethod, prefix: new HarmonyMethod(typeof(DoorPatcher), nameof(OpenCloseInteractionPrefix)));
        if (originalInteractionAllowedMethod != null)
            harmony.Patch(originalInteractionAllowedMethod, prefix: new HarmonyMethod(typeof(DoorPatcher), nameof(InteractionAllowedPrefix)));
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