using UnityEngine;

namespace GTFODoorMod.Temperature;

public class TemperaturePatches
{
    private static readonly Color TempColorBarLow = new(1, 0.8f, 0.32f);
    private static readonly Color TempColorBarHigh = new(1, 0.7f, 0f);
    
    public static void SetInfectionBarPostfix(ref SpriteRenderer bar, float val)
    {
        if (!TemperatureManager.IsTemperatureActive) return;
        
        bar.color = Color.Lerp(TempColorBarLow, TempColorBarHigh, val);
    }
    
    public static void UpdateInfectionPostfix(PUI_LocalPlayerStatus __instance, float infection, float infectionHealthRel)
    {
        if (!TemperatureManager.IsTemperatureActive) return;
        
        if (__instance.m_infectionText.enabled)
        {
            __instance.m_infectionText.text = $"OVERHEAT : {(object) Mathf.Floor(infection * 100f)}%";
            __instance.m_infectionText.color = Color.Lerp(TempColorBarLow, TempColorBarHigh, infection);
        }
    }
}