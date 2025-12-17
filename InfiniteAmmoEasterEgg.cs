using System;
using BepInEx.Logging;
using HarmonyLib;
using Player;
using SNetwork;

namespace GTFODoorMod;

public class InfiniteAmmoEasterEgg
{
    private static ManualLogSource logger;
    
    public InfiniteAmmoEasterEgg(Harmony harmony, ManualLogSource loggerParent) {
        logger = loggerParent;
        harmony.Patch(typeof(CommunicationMenu).GetMethod(nameof(CommunicationMenu.BroadcastDialog)), postfix: new HarmonyMethod(typeof(InfiniteAmmoEasterEgg), nameof(InfiniteAmmoPatch)));
        logger.LogInfo("Setup easter egg!");
    }

    // For Psycho, who always seems to run out of ammo
    static void InfiniteAmmoPatch(PlayerAgent src, uint textUID, PlayerAgent dst)
    {
        if (!GameStateManager.IsInExpedition)
        {
            return;
        }
        // Only allow on D3 - Breach
        if (!RundownManager.ActiveExpedition.Descriptive.PublicName.Equals("Breach"))
        {
            return;
        }
        uint needAmmoId = 1309;
        if (textUID == needAmmoId && src.PlayerName.Equals("PsychoMadEye", StringComparison.OrdinalIgnoreCase))
        {
            if (SNet.IsMaster)
            {
                var backpack = PlayerBackpackManager.GetBackpack(src.Owner);
                if (backpack != null)
                {
                    var ammoStorage = backpack.AmmoStorage;
                    ammoStorage.StandardAmmo.AddAmmo(ammoStorage.StandardAmmo.AmmoMaxCap);
                    ammoStorage.SpecialAmmo.AddAmmo(ammoStorage.SpecialAmmo.AmmoMaxCap);
                }
            }
            logger.LogInfo($"Gave easter egg ammo.");
        }
    }
}