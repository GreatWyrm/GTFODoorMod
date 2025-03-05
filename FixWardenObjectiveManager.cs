using BepInEx.Logging;
using HarmonyLib;

namespace GTFODoorMod;

public class FixWardenObjectiveManager
{
    private static ManualLogSource logger;

    public FixWardenObjectiveManager(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        harmony.Patch(typeof(WardenObjectiveManager).GetMethod(nameof(WardenObjectiveManager.OnLevelCleanup)), postfix: new HarmonyMethod(typeof(FixWardenObjectiveManager), nameof(OnLevelCleanupPostfix)));
        logger.LogInfo("Patched infection plane functions!");
    }

    // The value of m_exitEventsTriggered does not appear to be set to false at any point, so we need to manually fix it OnLevelCleanup
    static void OnLevelCleanupPostfix(WardenObjectiveManager __instance)
    {
        logger.LogInfo("Resetting value of exitEventsTriggered.");
        WardenObjectiveManager.m_exitEventsTriggered = false;
    }
}