using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFODoorMod.CustomWorldEvents;
using HarmonyLib;

namespace GTFODoorMod;

[BepInPlugin("com.giginss.doormod", "Giginss's Door Mod", "0.0.1")]
public class DoorPlugin : BasePlugin
{
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo("Plugin Giginss's Door Mod is loaded!");
        Log.LogInfo("Hello Complex!");

        var harmony = new Harmony("com.giginss.doormod");
        WorldEventsPatcher customEventsPatcher = new WorldEventsPatcher(harmony);
        DoorPatcher doorPatcher = new DoorPatcher(harmony);
    }
}
