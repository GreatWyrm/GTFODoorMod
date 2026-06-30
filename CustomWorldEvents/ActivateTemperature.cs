using GameData;
using GTFODoorMod.Temperature;

namespace GTFODoorMod.CustomWorldEvents;

public class ActivateTemperature : AbstractWorldEvent
{
    private TemperatureManager TemperatureManager;

    public ActivateTemperature(TemperatureManager temperatureManager)
    {
        this.TemperatureManager = temperatureManager;
    }
    
    public override System.String Identifier => "ActivateTemperature";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        TemperatureManager.ActivateTemperature(eData.Duration, eData.FogTransitionDuration);
    }
}