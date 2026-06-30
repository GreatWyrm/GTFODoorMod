using GameData;
using GTFODoorMod.Temperature;

namespace GTFODoorMod.CustomWorldEvents;

public class DeactivateTemperature : AbstractWorldEvent
{
    private TemperatureManager TemperatureManager;

    public DeactivateTemperature(TemperatureManager temperatureManager)
    {
        this.TemperatureManager = temperatureManager;
    }
    
    public override System.String Identifier => "DeactivateTemperature";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        TemperatureManager.DeactivateTemperature();
    }
    
}