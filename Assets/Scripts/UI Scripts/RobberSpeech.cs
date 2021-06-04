using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RobberSpeech : MonoBehaviour
{
    //Text
    public string[] robberTexts;
    private int index = 0;

    //Components
    private TMP_Text textMesh;
    public AudioSource robberVoice;

    //state variables
    private bool isPrinting = true;
    public bool isTalking = false;

    //Coroutine Variables
    public float waitTime = 5f;
    private bool isWaiting = false;

    // Character Coroutine
    public float characterTime = .1f;

    //Animation Variables
    private Animator anim;

    //Taking Money Scene Assets
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    public GameObject camera;
    public GameObject VaultLocation;

    //Button Variables
    public GameObject button;
    private bool clicked = false;

    // Start is called before the first frame update
    void Start()
    {
        //setting up textbox
        anim = GetComponent<Animator>();
        textMesh = GetComponentInChildren<TMP_Text>();

        //for inside bank
        if(camera == null)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        originalPosition = camera.transform.position;
        originalRotation = camera.transform.rotation;
    }

    public void PrintRobberText()
    {
        StartCoroutine(PrintStatementCo());
    }

    public void CheckEscape()
    {
        int numBars = DifficultyManager.goldBars;
        //zero bars
        if (numBars < 1)
        {
            index = 0;
        }
        //less than max bars
        else if(numBars < 11)
        {
            index = 5;
        }
        //over max bars
        else
        {
            index = 10;
        }
        PrintRobberText();
    }

    public void BackToBank()
    {
        if (!clicked)
        {
            anim.SetBool("isTalking", !anim.GetBool("isTalking"));
            clicked = true;
        }
    }

    public void GoToVault()
    {
        if(VaultLocation != null)
        {
            camera.transform.position = VaultLocation.transform.position;
            camera.transform.rotation = VaultLocation.transform.rotation;
            Debug.Log("Teleported To Vault!");
        }
        else
        {
            Debug.Log("Vault Location Not Set Yet");
        }

        if (button != null)
        {
            button.SetActive(true);
        }
    }

    public void ResetCamera()
    {
        if(button != null)
        {
            button.SetActive(false);
        }
        camera.transform.position = originalPosition;
        camera.transform.rotation = originalRotation;
    }

    public void LoadLevel(int index)
    {
        if(index == -1)
        {
            int newScene = SceneManager.GetActiveScene().buildIndex + 1;
            if (newScene >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadSceneAsync(0);
            }
            else
            {
                SceneManager.LoadSceneAsync(newScene);
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(index);
        }
    }

    public void GoToCutscene()
    {
        anim.SetBool("cutScene", true);
    }

    public void CleanText()
    {
        textMesh.text = "";
    }

    IEnumerator PrintStatementCo()
    {
        for(; index < robberTexts.Length; index++)
        {
            if(robberTexts[index] == "SKIP")
            {
                break;
            }
            textMesh.maxVisibleCharacters = 0;
            textMesh.text = robberTexts[index];
            while (textMesh.maxVisibleCharacters < textMesh.text.Length)
            {
                robberVoice.pitch = Random.Range(.75f, 1.25f);
                robberVoice.Play();
                textMesh.maxVisibleCharacters++;
                yield return new WaitForSeconds(characterTime);
            }
            yield return new WaitForSeconds(waitTime);
        }
        anim.SetBool("isTalking", false);
        index++;
    }
}
