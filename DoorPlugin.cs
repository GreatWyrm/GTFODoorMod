using BepInEx;
using BepInEx.Unity.IL2CPP;
using GTFODoorMod.CustomWorldEvents;

namespace GTFODoorMod;

[BepInPlugin("com.giginss.doormod", "Giginss's Door Mod", "0.0.1")]
public class DoorPlugin : BasePlugin
{
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo("Plugin Giginss's Door Mod is loaded!");
        Log.LogInfo("Hello Complex!");

        WorldEventsPatcher customEventsPatcher = new WorldEventsPatcher();
        DoorPatcher doorPatcher = new DoorPatcher();
    }
}
