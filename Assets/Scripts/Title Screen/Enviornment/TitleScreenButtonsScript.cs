using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenButtonsScript : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject creditItems;
    public GameObject backButton;
    public GameObject blackImage;

    public float creditsTime;

    private bool startedGame;

    public void BackButton()
    {
        StopAllCoroutines();
        titleScreen.SetActive(true);
        creditItems.SetActive(false);
        backButton.SetActive(false);
    }

    public void StartGame()
    {
        blackImage.SetActive(true);
        blackImage.GetComponent<Animator>().SetBool("GameStarted", true);
        StartCoroutine(LoadGame());
    }

    public void CreditsButton()
    {
        StartCoroutine(CreditsScroll());
        titleScreen.SetActive(false);
        creditItems.SetActive(true);
        backButton.SetActive(true);
    }

    IEnumerator CreditsScroll()
    {
        yield return new WaitForSeconds(creditsTime);
        BackButton();
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadSceneAsync(1);
    }
}
