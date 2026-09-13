using UnityEngine;

public class Npcneeds : MonoBehaviour
{
    [Range(0, 100)] public float hunger = 0f;
    [Range(0, 100)] public float sleep = 0f;
    [Range(0, 100)] public float energy = 100f;
    
    public float hungerGrowthPerSecond = 1f;
    public float sleepGrowthPerSecond = 0.5f;
    public float energyDrainPerSecond = 0.3f;

    
    void Update()
    {
        hunger += Time.deltaTime * hungerGrowthPerSecond;
        sleep += Time.deltaTime * sleepGrowthPerSecond;
        energy -= Time.deltaTime * energyDrainPerSecond;
        
        hunger =  Mathf.Clamp(hunger, 0f, 100f);
        sleep = Mathf.Clamp(sleep, 0f, 100f);
        energy = Mathf.Clamp(energy, 0f, 100f);
    }
    
    public void EatFood(float amount)
    {
        hunger -= amount;
        hunger = Mathf.Clamp(hunger, 0, 100);
    }

    public void SleepOff(float amount)
    {
        sleep -= amount;
        sleep = Mathf.Clamp(sleep, 0f, 100f);
    }
    
    public void RegenEnergy(float amount)
    {
        energy += amount;
        energy = Mathf.Clamp(energy, 0f, 100f);
    }
    
}