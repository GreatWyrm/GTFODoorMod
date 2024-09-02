using System.Collections.Generic;
using BepInEx.Logging;
using GameData;
using GTFO.API.Utilities;
using HarmonyLib;

namespace GTFODoorMod;


public class PabloHeavyHitreactPatch {

    private static ManualLogSource logger;

    public PabloHeavyHitreactPatch(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        var originalDamageMethod = typeof(Dam_EnemyDamageBase).GetMethod(nameof(Dam_EnemyDamageBase.ProcessReceivedDamage));
        harmony.Patch(originalDamageMethod, prefix: new HarmonyMethod(typeof(PabloHeavyHitreactPatch), nameof(HavePabloStagger)));
        logger.LogInfo("Patched damage received function!");
    }

    [HarmonyPrefix]
    static bool HavePabloStagger(Dam_EnemyDamageBase __instance, ref ES_HitreactType hitreact, ref bool tryForceHitreact, float damage, float staggerDamageMulti) {
        if (__instance.Owner.EnemyDataID == 47) {
            float totalStagger = __instance.m_damBuildToHitreact + damage * staggerDamageMulti;
            logger.LogInfo("Total Stagger: " + totalStagger);
            logger.LogInfo($"Damage: {damage}, Stagger Multi: {staggerDamageMulti}.");
            if (totalStagger >= __instance.Owner.EnemyBalancingData.Health.DamageUntilHitreact) {
                logger.LogInfo("Forcing a pablo stagger");
                hitreact = ES_HitreactType.Heavy;
                tryForceHitreact = true;
            }
        }
        return true;
    }
}