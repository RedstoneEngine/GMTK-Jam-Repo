using UnityEngine;
using UnityEngine.UI;

public class OxygenMeter : MonoBehaviour
{
    public Image CircleMeter;
    public float OxygenDepletionMultiplier;
    public static float Oxygen = 100f;
    public float MaxOxygenLevel = 100;

    private void Awake()
    {
        Oxygen = MaxOxygenLevel;
    }

    private void Update()
    {
        Oxygen -= Time.deltaTime * OxygenDepletionMultiplier;

        //Update UI
        CircleMeter.fillAmount = Oxygen / MaxOxygenLevel;
    }

    public void ReplenishOxygen()
    {
        Oxygen = MaxOxygenLevel;
    }

    //Call to this method from anywhere!
    public static void changeOxygen (float deduction)
    {
        Oxygen += deduction;
    }
}