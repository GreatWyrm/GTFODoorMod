using BepInEx.Logging;
using HarmonyLib;
using LevelGeneration;
using UnityEngine;

namespace GTFODoorMod;

public class OnlyBreakLocksPatch
{
    
    public OnlyBreakLocksPatch(Harmony harmony, ManualLogSource loggerParent) {
        var originalSetupMethod = typeof(LG_WeakResourceContainer).GetMethod(nameof(LG_WeakResourceContainer.SetupWeakLock));
        harmony.Patch(originalSetupMethod, prefix: new HarmonyMethod(typeof(OnlyBreakLocksPatch), nameof(SetBreakLock)));
        var originalBuilderMethod =
            typeof(LG_ResourceContainerBuilder).GetMethod(nameof(LG_ResourceContainerBuilder.SetupFunctionGO));
        harmony.Patch(originalBuilderMethod, prefix: new HarmonyMethod(typeof(OnlyBreakLocksPatch), nameof(ForceAllLocks)));
        loggerParent.LogInfo("Patched weak lock setup function!");
    }

    // Force the m_locked variable to true to ensure the container is locked
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LG_ResourceContainerBuilder), nameof(LG_ResourceContainerBuilder.SetupFunctionGO))]
    static bool ForceAllLocks(LG_ResourceContainerBuilder __instance, LG_LayerType layer, GameObject GO)
    {
        uint balanceId = RundownManager.ActiveExpeditionBalanceData.persistentID;
        if (balanceId == 5)
            __instance.m_locked = true;
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LG_WeakResourceContainer), nameof(LG_WeakResourceContainer.SetupWeakLock))]
    static bool SetBreakLock(LG_WeakResourceContainer __instance, ref eWeakLockType type)
    {
        uint balanceId = RundownManager.ActiveExpeditionBalanceData.persistentID;
        if (balanceId == 5)
        {
            type = eWeakLockType.Melee;
        }
        return true;
    }
}