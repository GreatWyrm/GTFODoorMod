using GameData;
using GTFODoorMod.Temperature;

namespace GTFODoorMod.CustomWorldEvents;

public class SetTemperatureBounds : AbstractWorldEvent
{

    private TemperatureManager temperatureManager;
    
    public SetTemperatureBounds(TemperatureManager manager)
    {
        this.temperatureManager = manager;
    }
    
    public override System.String Identifier => "SetTemperatureBounds";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        temperatureManager.SetTemperatureBounds(eData.Duration, eData.FogTransitionDuration);
    }
}