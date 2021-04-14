using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RespawnScreen : MonoBehaviour
{
    public TMP_Text respawnText;

    private void OnEnable()
    {
        PlayerHealth.OnDeath += DisplayRespawnScreen;
    }

    private void OnDisable()
    {
        PlayerHealth.OnDeath -= DisplayRespawnScreen;
    }

    public void DisplayRespawnScreen()
    {
        respawnText.enabled = true;
        respawnText.text = "You Died!\nPress Enter to respawn!";
    }
}
