using BepInEx.Logging;
using HarmonyLib;
using LevelGeneration;
using Player;

namespace GTFODoorMod;

public class FixDimensionInfectionPlane
{
    private static ManualLogSource logger;
    private static float lastLogtime;

    public FixDimensionInfectionPlane(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        var registerVolumeMethod = typeof(EffectVolumeManager).GetMethod(nameof(EffectVolumeManager.RegisterVolume));
        var unregisterVolumeMethod = typeof(EffectVolumeManager).GetMethod(nameof(EffectVolumeManager.UnregisterVolume));
        var updateMethod = typeof(EffectVolumeManager).GetMethod(nameof(EffectVolumeManager.Update));
        var blendMethodInfo = typeof(LocalPlayerAgentSettings).GetMethod(nameof(LocalPlayerAgentSettings.UpdateBlendTowardsTargetFogSetting));
        harmony.Patch(blendMethodInfo, postfix: new HarmonyMethod(typeof(FixDimensionInfectionPlane), nameof(FixInfectionPlanesInDimensions)));
        
        //harmony.Patch(registerVolumeMethod, postfix: new HarmonyMethod(typeof(InspectEffectVolumeManager), nameof(RegisterVolumePostfix)));
        //harmony.Patch(unregisterVolumeMethod, postfix: new HarmonyMethod(typeof(InspectEffectVolumeManager), nameof(UnregisterVolumePostfix)));
        //harmony.Patch(updateMethod, postfix: new HarmonyMethod(typeof(InspectEffectVolumeManager), nameof(UpdatePostfix)));
        logger.LogInfo("Patched blend method for dimensions!");
    }

    static void UpdatePostfix()
    {
        if (lastLogtime + 2f < Clock.Time)
        {
            lastLogtime = Clock.Time;
            logger.LogInfo($"Effect Volume Count: {EffectVolumeManager.volumes.Count}.");
            var infectPlane = LocalPlayerAgentSettings.Current.infectionPlane;
            logger.LogInfo($"Local Infection Plane details; Highest Altitude: {infectPlane.highestAltitude}, Lowest Altitude: {infectPlane.lowestAltitude}.");

            for (int i = 0; i < EffectVolumeManager.targets.Count; i++)
            {
                IEffectVolumeTarget target = EffectVolumeManager.targets[i];
                logger.LogInfo($"Effect Volume Target Data Ref Pos: {target.EffectVolumeTargetData.referencePosition.ToString()}.");
            }
            for (int i = 0; i < Dimension.DimensionCount; i++)
            {
                bool success = Dimension.GetDimensionFromCreationIndex(i, out Dimension dimension);
                logger.LogInfo($"Dimension {dimension.CreationOrderIndex}, GroundY: {dimension.GroundY}, Position: {dimension.Position}.");
            }
        }
    }
    
    static void RegisterVolumePostfix(EffectVolume volume)
    {
        logger.LogInfo($"Registered Effect Volume {volume.GetType().FullName}.");
        logger.LogInfo($"Effect Volume Details; Contents: {volume.contents}, EffectOrder: {volume.effectOrder}, Invert: {volume.invert}, Modification: {volume.modification}, ModificationScale: {volume.modificationScale}.");
        if (volume is EV_Plane)
        {
            EV_Plane plane = (EV_Plane)volume;
            logger.LogInfo($"EV_Plane details; Highest Altitude: {plane.highestAltitude}, Lowest Altitude: {plane.lowestAltitude}.");
        } else if (volume is EV_Sphere)
        {
            EV_Sphere sphere = (EV_Sphere)volume;
            logger.LogInfo($"EV_Sphere details; Max Radius: {sphere.maxRadius}, Min Radius: {sphere.minRadius}, Position: {sphere.position}.");
        }
    }
    
    static void UnregisterVolumePostfix(EffectVolume volume)
    {
        logger.LogInfo($"Unregistered Effect Volume {volume.GetType().FullName}.");
        logger.LogInfo($"Effect Volume Details; Contents: {volume.contents}, EffectOrder: {volume.effectOrder}, Invert: {volume.invert}, Modification: {volume.modification}, ModificationScale: {volume.modificationScale}.");
        if (volume is EV_Plane)
        {
            EV_Plane plane = (EV_Plane)volume;
            logger.LogInfo($"EV_Plane details; Highest Altitude: {plane.highestAltitude}, Lowest Altitude: {plane.lowestAltitude}.");
        } else if (volume is EV_Sphere)
        {
            EV_Sphere sphere = (EV_Sphere)volume;
            logger.LogInfo($"EV_Sphere details; Max Radius: {sphere.maxRadius}, Min Radius: {sphere.minRadius}, Position: {sphere.position}.");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalPlayerAgentSettings),
        nameof(LocalPlayerAgentSettings.UpdateBlendTowardsTargetFogSetting))]
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
}