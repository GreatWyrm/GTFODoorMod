using CellMenu;
using Player;
using SNetwork;
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
    
    public static void UpdatePlayerInventoryPostfix(CM_PageMap __instance, ref SNet_Player player, int count)
    {
        if (!TemperatureManager.IsTemperatureActive) return;
        
        if (!player.IsInSlot)
            return;
        int index = player.PlayerSlotIndex();
        if (PlayerManager.TryGetPlayerAgent(ref index, out PlayerAgent playerAgent))
        {
            string posttext = $" <color=orange>({(playerAgent.Damage.GetHealthRel() * 100f).ToString("N0")}%)</color>";
            if (playerAgent.Damage.Infection > 0.1f)
            {
                posttext += $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.Lerp(TempColorBarLow, TempColorBarHigh, playerAgent.Damage.Infection))}>({Mathf.Floor(playerAgent.Damage.Infection * 100f)}%)</color>";
            }
                
            __instance.m_inventory[index].SetHeader(player.NickName + posttext, player.PlayerColor);
        }
    }
}