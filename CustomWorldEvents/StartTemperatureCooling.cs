using GameData;
using GTFODoorMod.Temperature;

namespace GTFODoorMod.CustomWorldEvents;

public class StartTemperatureCooling : AbstractWorldEvent
{
    private TemperatureManager TemperatureManager;

    public StartTemperatureCooling(TemperatureManager temperatureManager)
    {
        this.TemperatureManager = temperatureManager;
    }
    
    public override System.String Identifier => "StartTemperatureCooling";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        TemperatureManager.AddTemporaryCooling(eData.Duration, eData.FogTransitionDuration);
    }
}