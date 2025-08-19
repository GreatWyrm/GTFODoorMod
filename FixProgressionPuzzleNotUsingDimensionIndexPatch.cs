using BepInEx.Logging;
using GameData;
using HarmonyLib;
using LevelGeneration;

namespace GTFODoorMod;

public class FixProgressionPuzzleNotUsingDimensionIndexPatch
{
    private static ManualLogSource logger;

    public FixProgressionPuzzleNotUsingDimensionIndexPatch(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        harmony.Patch(typeof(LG_Distribute_ProgressionPuzzles).GetMethod(nameof(LG_Distribute_ProgressionPuzzles.CreateKeyItemDistribution)), prefix: new HarmonyMethod(typeof(FixProgressionPuzzleNotUsingDimensionIndexPatch), nameof(TestPrefix)));
        logger.LogInfo("Patched infection plane functions!");
    }

    
    static bool TestPrefix(LG_Distribute_ProgressionPuzzles __instance, GateKeyItem keyItem, ZonePlacementData placementData)
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
            __instance.m_layer = dimension.MainLayer;
            logger.LogInfo($"Replaced with {dimension.DimensionIndex}'s MainLayer.");
        }
        // Otherwise no mismatch, no need to run
        return true;
    }
}