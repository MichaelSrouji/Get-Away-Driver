using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour
{
    public static int goldBars = 0;
    private int maxGoldBars = 12;
    public float difficultyPercent;
    public Image activeCopBar;
    public float fillRate = 10f;
    public float targetFill;

    void Start()
    {
        goldBars = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "money")
        {
            goldBars++; 
            difficultyPercent = ((float)goldBars)/maxGoldBars;
            // activeCopBar.fillAmount = difficultyPercent;
            targetFill = difficultyPercent;
            StopAllCoroutines();
            StartCoroutine(LerpCopFill());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "money")
        {
            goldBars--; 
            difficultyPercent = ((float)goldBars)/maxGoldBars;
            // activeCopBar.fillAmount = difficultyPercent;
            targetFill = difficultyPercent;
            StopAllCoroutines();
            StartCoroutine(LerpCopFill());
        }
    }

    IEnumerator LerpCopFill()
    {
        while (activeCopBar.fillAmount != targetFill)
        {
            activeCopBar.fillAmount = Mathf.Lerp(activeCopBar.fillAmount, targetFill, Time.deltaTime * fillRate);
            yield return null;
        }
    }

}
