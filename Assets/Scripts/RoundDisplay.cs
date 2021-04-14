using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundDisplay : MonoBehaviour
{
    public TMP_Text roundText;

    private void OnEnable()
    {
        RoundHandler.roundBegun += RoundBegin;
        RoundHandler.roundEnded += RoundEnd;
        RoundHandler.gameBegun += GameBegin;
    }

    private void OnDisable()
    {
        RoundHandler.roundBegun -= RoundBegin;
        RoundHandler.roundEnded -= RoundEnd;
        RoundHandler.gameBegun -= GameBegin;
    }

    public void RoundBegin(int round, int waitTime)
    {
        StartCoroutine(DisplayRoundBegin(round, waitTime));
    }

    public void RoundEnd(int round, int waitTime)
    {
        StartCoroutine(DisplayRoundEnd(waitTime));
    }

    public void GameBegin(int countdown)
    {
        StartCoroutine(DisplayGameBegin(countdown));
    }

    private IEnumerator DisplayRoundBegin(int round, int waitTime)
    {
        roundText.enabled = true;
        roundText.text = "Round " + round;

        yield return new WaitForSeconds(waitTime);
        roundText.enabled = false;
    }

    private IEnumerator DisplayRoundEnd(int waitTime)
    {
        roundText.enabled = true;
        roundText.text = "Round complete!";

        yield return new WaitForSeconds(waitTime);

        roundText.enabled = true;
    }

    private IEnumerator DisplayGameBegin(int countdown)
    {
        roundText.enabled = true;
        for (int i = countdown; i > 0; i--)
        {
            roundText.text = "Game starting in " + i;
            yield return new WaitForSeconds(1);
        }
    }
}
