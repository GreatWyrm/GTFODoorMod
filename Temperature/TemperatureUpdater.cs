using System;
using Player;
using UnityEngine;

namespace GTFODoorMod.Temperature;

public class TemperatureUpdater : MonoBehaviour
{
    
    private float tempRate;
    private float currentTemp;
    private bool currentlyTicking;

    private readonly float TEMP_SYNC_TIMER = 10f;
    private float currentSyncTimer;
    private readonly float TEMP_INFECTION_RATE = 0.02f;
    // Thresholds
    private readonly Color TEMP_OKAY_COLOR = new(0.5f, 0.5f, 0.5f);
    private readonly float TEMP_WARN_THRESHOLD = 37f;
    private readonly Color TEMP_WARN_THRESHOLD_COLOR = Color.yellow;
    private readonly float TEMP_DAMAGE_THRESHOLD = 40f;
    private readonly Color TEMP_DAMAGE_THRESHOLD_COLOR = Color.red;
    
    // Callbacks
    private Action<float, Color, string> displayCallback;
    private Action<float> syncCallback;

    public void SetupCallbacks(Action<float, Color, string> displayCallback, Action<float> syncCallback)
    {
        this.displayCallback = displayCallback;
        this.syncCallback = syncCallback;
    }

    public void StartTemperatureTicking(float startTemp, float tempRate)
    {
        this.currentTemp = startTemp;
        this.tempRate = tempRate;
        this.currentlyTicking = true;
    }
    
    public void StopTemperatureTicking()
    {
        currentlyTicking = false;
    }

    public void SetTemperatureFromSync(float temp)
    {
        currentTemp = temp;
    }
    
    private void Update()
    {
        if (currentlyTicking)
        {
            float delta = Time.deltaTime;
            currentTemp += tempRate * delta;
            currentSyncTimer += delta;
            if (currentSyncTimer >= TEMP_SYNC_TIMER)
            {
                currentSyncTimer = 0f;
                syncCallback.Invoke(currentTemp);
            }

            if (currentTemp >= TEMP_DAMAGE_THRESHOLD)
            {
                float infectAmount = delta * TEMP_INFECTION_RATE;
                PlayerAgent playerAgent = PlayerManager.GetLocalPlayerAgent();
                playerAgent.Damage.ModifyInfection(new pInfection
                {
                    amount = infectAmount,
                    effect = pInfectionEffect.None,
                    mode = pInfectionMode.Add
                }, false, false);
                displayCallback.Invoke(currentTemp, TEMP_DAMAGE_THRESHOLD_COLOR, "OVERHEATING");
            } else if (currentTemp >= TEMP_WARN_THRESHOLD)
            {
                float healAmount = delta * TEMP_INFECTION_RATE * -1;
                PlayerAgent playerAgent = PlayerManager.GetLocalPlayerAgent();
                playerAgent.Damage.ModifyInfection(new pInfection
                {
                    amount = healAmount,
                    effect = pInfectionEffect.None,
                    mode = pInfectionMode.Add
                }, false, false);
                displayCallback.Invoke(currentTemp, TEMP_WARN_THRESHOLD_COLOR, "WARMING");
            }
            else
            {
                float healAmount = delta * TEMP_INFECTION_RATE * -1;
                PlayerAgent playerAgent = PlayerManager.GetLocalPlayerAgent();
                playerAgent.Damage.ModifyInfection(new pInfection
                {
                    amount = healAmount,
                    effect = pInfectionEffect.None,
                    mode = pInfectionMode.Add
                }, false, false);
                displayCallback.Invoke(currentTemp, TEMP_OKAY_COLOR, "ACCEPTABLE");
            }
        }
    }
    
    
}