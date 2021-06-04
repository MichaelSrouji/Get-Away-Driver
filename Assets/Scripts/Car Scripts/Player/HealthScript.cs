using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    private Image healthBar;
    public float healthDecreaseRate = 10f;
    private float trueHealth = 1f;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
    }

    public void DecreaseHealth(float currentHealth, float healthLoss)
    {
        trueHealth = currentHealth - healthLoss;
        StopAllCoroutines();
        StartCoroutine(LerpHealth(trueHealth));
    }

    IEnumerator LerpHealth(float targetHealth)
    {
        while (healthBar.fillAmount > targetHealth || healthBar.fillAmount <= 0)
        {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, trueHealth, Time.deltaTime * healthDecreaseRate);
            yield return new WaitForEndOfFrame();
        }
    }
}
