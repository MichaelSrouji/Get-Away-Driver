using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopCarDetectionBar : MonoBehaviour
{
    public Image copCarsImage;
    public float fillRate = .1f;
    private float targetFill;

    public void LoseCopCar()
    {
        if(targetFill - .2f >= 0)
        {
            targetFill -= .2f;
            StopAllCoroutines();
            StartCoroutine(LerpCopFill());
        }
    }

    public void NewCopCar()
    {
        if (copCarsImage.fillAmount + .2f <= 1)
        {
            targetFill += .2f;
            StopAllCoroutines();
            StartCoroutine(LerpCopFill());
        }
    }

    IEnumerator LerpCopFill()
    {
        while (copCarsImage.fillAmount != targetFill)
        {
            copCarsImage.fillAmount = Mathf.Lerp(copCarsImage.fillAmount, targetFill, Time.deltaTime * fillRate);
            yield return null;
        }
        Debug.Log("finished fill");
    }
}
