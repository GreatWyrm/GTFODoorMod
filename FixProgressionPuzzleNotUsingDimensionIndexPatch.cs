using BepInEx.Logging;
using GameData;
using HarmonyLib;
using LevelGeneration;

namespace GTFODoorMod;

public class FixProgressionPuzzleNotUsingDimensionIndexPatch
{
    private static ManualLogSource logger;
    private static LG_Layer lgLayer;

    public FixProgressionPuzzleNotUsingDimensionIndexPatch(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        harmony.Patch(typeof(LG_Distribute_ProgressionPuzzles).GetMethod(nameof(LG_Distribute_ProgressionPuzzles.CreateKeyItemDistribution)), 
            prefix: new HarmonyMethod(typeof(FixProgressionPuzzleNotUsingDimensionIndexPatch), nameof(OverrideLayerIfNeededPrefix)),
            postfix: new HarmonyMethod(typeof(FixProgressionPuzzleNotUsingDimensionIndexPatch), nameof(ResetProgressionPuzzleMainLayer)));
        logger.LogInfo("Patched progression puzzle distribution function!");
    }
    
    static bool OverrideLayerIfNeededPrefix(LG_Distribute_ProgressionPuzzles __instance, GateKeyItem keyItem, ZonePlacementData placementData)
    {
        if (__instance.m_layer.m_dimension.DimensionIndex != placementData.DimensionIndex)
        {
            logger.LogInfo($"Found dimension mismatch between layer dimension and placement data dimension for Keycard {keyItem.m_keyName}, fixing...");
            Dimension.GetDimension(placementData.DimensionIndex, out Dimension dimension);
            if (dimension == null)
            {
                logger.LogError($"Dimension {placementData.DimensionIndex} not found! Aborting...");
                return true;
            }

            if (dimension.MainLayer.m_zones._size <= (int)placementData.LocalIndex)
            {
                logger.LogInfo($"Cannot place keycard in {dimension.DimensionIndex}! Not enough zones, dimension has {dimension.MainLayer.m_zones._size} zones and Placement Index is {placementData.LocalIndex}. Aborting...");
                return true;
            }
            // Store the current layer to reset afterwards
            lgLayer = __instance.m_layer;
            __instance.m_layer = dimension.MainLayer;
            logger.LogInfo($"Replaced with {dimension.DimensionIndex}'s MainLayer.");
        }
        // Otherwise no mismatch, no need to run
        return true;
    }

    // Need to reset the value of LG_Distribute_ProgressionPuzzles layer to its original value, otherwise it will break due to iterating through the dimension's zones that we substituted in
    static void ResetProgressionPuzzleMainLayer(LG_Distribute_ProgressionPuzzles __instance)
    {
        if (lgLayer != null)
        {
            logger.LogInfo($"Resetting LG_Layer in Distribute Progression Puzzles to Dimension {lgLayer.m_dimension.DimensionIndex}.");
            __instance.m_layer = lgLayer;
            lgLayer = null;
        }
    }
}