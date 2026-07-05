using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class ForceSolveUplink : AbstractWorldEvent
{
    public override System.String Identifier => "ForceSolveUplink";

    public override void OnEventTrigger(ref WardenObjectiveEventData eData)
    {
        if (TryGetZone(eData, out var zone))
        {
            bool foundUplinkTerminal = false;
            foreach (var terminal in zone.TerminalsSpawnedInZone)
            {
                if (terminal.UplinkPuzzle.Connected)
                {
                    foundUplinkTerminal = true;
                    terminal.UplinkPuzzle.Solved = true;
                    terminal.UplinkPuzzle.UpdateGUI();
                    terminal.AddLine(TerminalLineType.Warning, "Forcefully terminating uplink", 3f);
                    if (terminal.UplinkPuzzle.OnPuzzleSolved != null)
                    {
                        terminal.UplinkPuzzle.OnPuzzleSolved.Invoke();
                    }
                }
            }

            if (!foundUplinkTerminal)
            {
                eventLogger.LogError($"Failed to find any active uplink terminal in {zone.m_navInfo.GetFormattedText(LG_NavInfoFormat.Full_And_Number_With_Space)}");
            }
        }
        else
        {
            eventLogger.LogError("Failed to find zone for ForceSolveUplink!");
        }
    }
}