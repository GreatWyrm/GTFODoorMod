using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFODoorMod.CustomWorldEvents;
using HarmonyLib;
using UnityEngine;

namespace GTFODoorMod;

[BepInPlugin("com.giginss.doormod", "Giginss's Door Mod", "0.0.3")]
public class DoorPlugin : BasePlugin
{
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo("Plugin Giginss's Door Mod is loading!");
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

        var harmony = new Harmony("com.giginss.doormod");
        WorldEventsPatcher customEventsPatcher = new WorldEventsPatcher(harmony, redXTexture);
        DoorPatcher doorPatcher = new DoorPatcher(harmony);
        PabloHeavyHitreactPatch pabloPatcher = new PabloHeavyHitreactPatch(harmony, Log);  
    }
}
