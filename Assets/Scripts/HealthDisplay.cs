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

    private void OnEnable()
    {
        PlayerHealth.OnHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChange -= UpdateHealth;
    }

    public void UpdateHealth(int health, int maxHealth)
    {
        healthText.text = health.ToString();
        healthFill.fillAmount = (float)health / maxHealth;

        if (useHSVColor)
        {
            float value = GetHSV(health, maxHealth, 0, 125, 0);
            healthFill.color = Color.HSVToRGB(value / 360, 1.0f, 1.0f);
        }
    }

    private static float GetHSV(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}
