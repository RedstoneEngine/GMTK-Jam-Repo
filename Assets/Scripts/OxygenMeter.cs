using UnityEngine;

public class OxygenMeter : MonoBehaviour
{
    public float OxygenDepletionMultiplier;
    public float Oxygen = 100f;
    public float MaxOxygenLevel = 100;

    private void Awake()
    {
        Oxygen = MaxOxygenLevel;
    }

    private void Update()
    {
        Oxygen -= Time.deltaTime * OxygenDepletionMultiplier;
    }

    public void ReplenishOxygen()
    {
        Oxygen = MaxOxygenLevel;
    }
}