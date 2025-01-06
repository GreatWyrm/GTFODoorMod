
using BepInEx.Logging;
using GameData;
using HarmonyLib;
using Player;

namespace GTFODoorMod;

public class AllowNegativeInfectionFogPatch
{
    private static ManualLogSource logger;

    public AllowNegativeInfectionFogPatch(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        var blendMethodInfo = typeof(LocalPlayerAgentSettings).GetMethod(nameof(LocalPlayerAgentSettings.UpdateBlendTowardsTargetFogSetting));
        harmony.Patch(blendMethodInfo, postfix: new HarmonyMethod(typeof(AllowNegativeInfectionFogPatch), nameof(AllowBlendNegativeInfectionPlane)));
        harmony.Patch(typeof(LocalPlayerAgentSettings).GetMethod(nameof(LocalPlayerAgentSettings.SetFogSettings)), postfix: new HarmonyMethod(typeof(AllowNegativeInfectionFogPatch), nameof(AllowSetNegativeInfectionPlane)));
        logger.LogInfo("Patched infection plane functions!");
    }

    static void AllowSetNegativeInfectionPlane(LocalPlayerAgentSettings __instance, ref FogSettingsDataBlock fogSettings)
    {
        // Instance is null because of LocalPlayerAgentSettings is technically static
        if (fogSettings is not null)
        {
            if (fogSettings.Infection < 0.0)
            {
                // Need to manually register the negative infection plane
                logger.LogInfo("Set infection plane to one that has negative infection.");
                LocalPlayerAgentSettings.Current.infectionPlane.invert = fogSettings.DensityHeightMaxBoost > (double) fogSettings.FogDensity;
                LocalPlayerAgentSettings.Current.infectionPlane.contents = eEffectVolumeContents.Infection;
                LocalPlayerAgentSettings.Current.infectionPlane.modification = eEffectVolumeModification.Inflict;
                LocalPlayerAgentSettings.Current.infectionPlane.modificationScale = fogSettings.Infection;
                LocalPlayerAgentSettings.Current.infectionPlane.lowestAltitude = fogSettings.DensityHeightAltitude;
                LocalPlayerAgentSettings.Current.infectionPlane.highestAltitude = fogSettings.DensityHeightAltitude + fogSettings.DensityHeightRange;
                EffectVolumeManager.RegisterVolume(LocalPlayerAgentSettings.Current.infectionPlane);
                LocalPlayerAgentSettings.Current.isInfectionPlaneRegistered = true;
            }
        }
    }

    // TODO: Confirm this works
    static void AllowBlendNegativeInfectionPlane(LocalPlayerAgentSettings __instance, float amount)
    {
        float currentInfection = LocalPlayerAgentSettings.Current.currentInfection;
        if (currentInfection < 0.0 && !LocalPlayerAgentSettings.Current.isInfectionPlaneRegistered)
        {
            // Manually re-register negative infection plane
            EffectVolumeManager.RegisterVolume(LocalPlayerAgentSettings.Current.infectionPlane);
            LocalPlayerAgentSettings.Current.isInfectionPlaneRegistered = true;
        }
    }
}