using GameData;
using LevelGeneration;

namespace GTFODoorMod.CustomWorldEvents;

public class ChangeMainObjectiveText : AbstractWorldEvent
{
    public override string Identifier => "ChangeMainObjectiveText";

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        var wardenObjectives = GuiManager.PlayerLayer.WardenObjectives;
        string newHeaderText = wardenObjectives.StyleMainObjText(WardenObjectiveManager.ReplaceFragmentsInString(LG_LayerType.MainLayer, 0, eventData.CustomSubObjectiveHeader.ToString()), true);
        string newDescriptionText = wardenObjectives.StyleSubObjText(WardenObjectiveManager.ReplaceFragmentsInString(LG_LayerType.MainLayer, 0, eventData.CustomSubObjective.ToString()), PUI_GameObjectives.SubObjectiveStyleType.Normal);
        var progressionObjective = wardenObjectives.m_progressionObjectiveMap[PUI_GameObjectives.MAIN_OBJECTIVE_ID];
        if (progressionObjective != null)
        {
            progressionObjective.m_header.text = newHeaderText;
            progressionObjective.m_text.text = newDescriptionText;
            progressionObjective.ResizeAccordingToText();
            if (!eventData.Enabled)
            {
                CoroutineManager.BlinkIn(progressionObjective.Header, 0.1f);
                CoroutineManager.BlinkIn(progressionObjective.SubObjective, 0.3f);
            }
        }
        else
        {
            eventLogger.LogWarning("Failed to find Main Objective Progression Objective!");
        }
    }
}