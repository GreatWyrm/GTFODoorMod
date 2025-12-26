using BepInEx.Logging;
using HarmonyLib;
using LevelGeneration;
using Player;

namespace GTFODoorMod;

public class FixBaseGameBugs
{
    private static ManualLogSource logger;
    private static float lastLogtime;

    public FixBaseGameBugs(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        var blendMethodInfo = typeof(LocalPlayerAgentSettings).GetMethod(nameof(LocalPlayerAgentSettings.UpdateBlendTowardsTargetFogSetting));
        harmony.Patch(blendMethodInfo, postfix: new HarmonyMethod(typeof(FixBaseGameBugs), nameof(FixInfectionPlanesInDimensions)));
        harmony.Patch(typeof(LG_LightCollection).GetMethod(nameof(LG_LightCollection.ResetUpdateValues)),
            prefix: new HarmonyMethod(typeof(FixBaseGameBugs), nameof(FixEmptyLightCollection)));
        logger.LogInfo("Patched blend method for dimensions and for empty light collection!");
    }
    
    /**
     * Base game bug: The infection plane in dimensions other than reality appears to ahve the dimension groundY parameter added twice when it should only be added once
     * Fix: Subtract the groundY if we are in another dimension
     */
    static void FixInfectionPlanesInDimensions(LocalPlayerAgentSettings __instance, float amount)
    {
        PlayerAgent playerAgent = PlayerManager.GetLocalPlayerAgent();
        if (playerAgent != null)
        {
            Dimension dimension = playerAgent.Dimension;
            if (dimension == null || dimension.IsMainDimension)
                return;
            
            float toSubtract = dimension.GroundY;
            
            LocalPlayerAgentSettings.Current.infectionPlane.highestAltitude -= toSubtract;
            LocalPlayerAgentSettings.Current.infectionPlane.lowestAltitude -= toSubtract;
        }
    }

    /**
     * Base game bug: LG_LightCollection can sometimes have 0 lights (occurs with 2 reactors in 2 layers, likely due to the first reactor setting search flags so the second can't find lights)
     * This will throw an index out of range when trying to progress from Reactor Intensive Test -> Verify, and softlock the reactor
     * Fix: Skip the problematic index code path
     */
    static bool FixEmptyLightCollection(LG_LightCollection __instance, bool toStart)
    {
        if (__instance.collectedLights.Count == 0 && !toStart)
        {
            logger.LogWarning("Light collection has an empty light array! Skipping Reset...");
            return false;
        }

        return true;
    }
}