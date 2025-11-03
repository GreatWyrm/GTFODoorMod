using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFODoorMod.CustomWorldEvents;
using HarmonyLib;
using SNetwork;
using UnityEngine;

namespace GTFODoorMod;

[BepInPlugin("com.giginss.rundownmod", "Giginss's Rundown Mod", "0.0.4")]
public class DoorPlugin : BasePlugin
{
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo("Plugin Giginss's Rundown Mod is loading!");
        Log.LogInfo("Hello Complex!");
        
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
        var originalMethod = typeof(SNet_Replication).GetMethod(nameof(SNet_Replication.AllocateKey), types: new [] { typeof(SNet_ReplicatorType), typeof(ushort) });
        harmony.Patch(originalMethod, new HarmonyMethod(typeof(ReplicationPatch), nameof(ReplicationPatch.Prefix)));
    }
}
