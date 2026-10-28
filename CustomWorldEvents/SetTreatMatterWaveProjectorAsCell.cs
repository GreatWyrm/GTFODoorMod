using GameData;
using Gear;
using GTFO.API;
using HarmonyLib;

namespace GTFODoorMod.CustomWorldEvents;

public class SetTreatMatterWaveProjectorAsCell : AbstractWorldEvent
{
    public override System.String Identifier => "SetTreatMatterWaveProjectorAsCell";

    private static bool state = false;

    public SetTreatMatterWaveProjectorAsCell(Harmony harmony)
    {
        // Apply patch
        var originalEvent = typeof(CarryItemEquippableFirstPerson).GetMethod(nameof(CarryItemEquippableFirstPerson.Setup));
        harmony.Patch(originalEvent, postfix: new HarmonyMethod(typeof(SetTreatMatterWaveProjectorAsCell), nameof(SetupPostfix)));
        
        // Reset on level done
        LevelAPI.OnLevelCleanup += ResetState;
    }

    public override void OnEventTrigger(ref WardenObjectiveEventData eventData)
    {
        state = eventData.Enabled;
    }

    private void ResetState()
    {
        state = false;
    }

    private static void SetupPostfix(CarryItemEquippableFirstPerson __instance, ItemDataBlock data)
    {
        if (!state)
        {
            return;
        }

        if (data.persistentID == 164 || data.persistentID == 166)
        {
            eventLogger.LogInfo("Picked up Matter Wave Projector while state was true, setting to powercell insert type");
            __instance.m_insertCheckType = eCarryItemInsertTargetType.PowerCell;
        }
    }
}