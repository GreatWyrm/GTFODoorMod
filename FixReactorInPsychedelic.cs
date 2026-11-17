using BepInEx.Logging;
using HarmonyLib;
using LevelGeneration;

namespace GTFODoorMod;

public class FixReactorInPsychedelic
{
    
    private static ManualLogSource logger;

    public FixReactorInPsychedelic(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        harmony.Patch(typeof(LG_WardenObjective_Reactor).GetMethod(nameof(LG_WardenObjective_Reactor.OnBuildDone)),
            prefix: new HarmonyMethod(typeof(FixReactorInPsychedelic), nameof(ReactorLogging)));
    }

    private static bool ReactorLogging(LG_WardenObjective_Reactor __instance)
    {
        if (RundownManager.ActiveExpedition.Descriptive.PublicName.Equals("Psychedelic"))
        {
            logger.LogInfo("It's EX, apply the SpawnNode fix to the reactor");
            var component = __instance.m_terminal.GetComponent<iLG_SpawnedInNodeHandler>();
            if (component.SpawnNode == null)
            {
                logger.LogInfo($"SpawnNode is null, setting...");
                component.SpawnNode = __instance.SpawnNode;
            }
        }
        return true;
    }
}