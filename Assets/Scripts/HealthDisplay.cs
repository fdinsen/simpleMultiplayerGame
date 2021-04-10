using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthFill;

    [SerializeField] private bool useHSVColor = false;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        if(healthFill == null)
        {
            Debug.LogError("Health Fill is not set to a health bar in HealthDisplay.");
        }
        if(healthText == null)
        {
            Debug.LogError("Health Text is not set to a health TextMeshPro in HealthDisplay");
        }
    }

    private void FixedUpdate()
    {
        if(healthFill != null && healthText != null)
        {
            healthText.text = playerHealth.GetHealth().ToString();
            healthFill.fillAmount = (float)playerHealth.GetHealth() / playerHealth.GetMaxHealth();

            if (useHSVColor)
            {
                float value = GetHSV(playerHealth.GetHealth(), playerHealth.GetMaxHealth(), 0, 125, 0);
                healthFill.color = Color.HSVToRGB(value / 360, 1.0f, 1.0f);
            }
        }
    }

    private static float GetHSV(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
