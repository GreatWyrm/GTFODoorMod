using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using GameData;
using GTFO.API;
using GTFODoorMod.CustomWorldEvents;
using HarmonyLib;
using Player;
using SNetwork;
using UnityEngine;

namespace GTFODoorMod;

[BepInPlugin("com.giginss.rundownmod", "Giginss's Rundown Mod", "1.0.0")]
public class DoorPlugin : BasePlugin
{
 
    private ConfigEntry<bool> lurkerNerf;
    private ConfigEntry<bool> psychoBuff;
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo("Plugin Giginss's Rundown Mod is loading!");
        Log.LogInfo("Hello Complex!");
        
        // Config Loading
        lurkerNerf = Config.Bind("Easter Egg", "LurkerNerf", true, "Easter Egg to prevent anyone named Lurker from picking a hammer");
        psychoBuff = Config.Bind("Easter Egg", "PsychoBuff", true, "Easter Egg to allow anyone named PsychoMadEye to gain enough ammo on D3.");
        
        // Load in image
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GTFODoorMod.resources.red-x.png");
        Texture2D redXTexture = new Texture2D(2, 2);
        if (stream != null)
        {
            byte[] buffer = new byte[32 * 1024]; // Space for a 32kb image
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                byte[] imageData = ms.ToArray();
                ImageConversion.LoadImage(redXTexture, imageData);
            }
        }

        var harmony = new Harmony("com.giginss.rundownmod");
        WorldEventsPatcher customEventsPatcher = new WorldEventsPatcher(harmony, redXTexture);
        DoorPatcher doorPatcher = new DoorPatcher(harmony);
        PabloHeavyHitreactPatch pabloPatcher = new PabloHeavyHitreactPatch(harmony, Log);
        AllowNegativeInfectionFogPatch fogPatch = new AllowNegativeInfectionFogPatch(harmony, Log);
        FixWardenObjectiveManager fixWardenObjectiveManager = new FixWardenObjectiveManager(harmony, Log);
        OnlyBreakLocksPatch onlyBreakLocksPatch = new OnlyBreakLocksPatch(harmony, Log);
        FixProgressionPuzzleNotUsingDimensionIndexPatch dimensionIndexPatch = new FixProgressionPuzzleNotUsingDimensionIndexPatch(harmony, Log);
        FixDimensionInfectionPlane fixDimensionInfectionPlane = new FixDimensionInfectionPlane(harmony, Log);
        if (psychoBuff.Value)
        {
            InfiniteAmmoEasterEgg infiniteAmmoEasterEgg = new InfiniteAmmoEasterEgg(harmony, Log);
        }

        if (lurkerNerf.Value)
        {
            Log.LogInfo($"Lurker Nerf is enabled, adding function to OnManagersSetup.");
            EventAPI.OnManagersSetup += CheckAndDisableHammers;
        }
        var originalMethod = typeof(SNet_Replication).GetMethod(nameof(SNet_Replication.AllocateKey), types: new [] { typeof(SNet_ReplicatorType), typeof(ushort) });
        harmony.Patch(originalMethod, new HarmonyMethod(typeof(ReplicationPatch), nameof(ReplicationPatch.Prefix)));
    }
    
    private static void CheckAndDisableHammers() 
    {
        if (SteamManager.LocalPlayerName.Equals("Lurker", StringComparison.OrdinalIgnoreCase))
        {
            PlayerOfflineGearDataBlock.GetBlock(9).internalEnabled = false;
            PlayerOfflineGearDataBlock.GetBlock(26).internalEnabled = false;
            PlayerOfflineGearDataBlock.GetBlock(27).internalEnabled = false;
            PlayerOfflineGearDataBlock.GetBlock(28).internalEnabled = false;
            PlayerOfflineGearDataBlock.GetBlock(57).internalEnabled = false;
        }
    }
}
