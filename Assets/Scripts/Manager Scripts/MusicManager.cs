using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip raw;
    public AudioClip metalica;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] allMusic = GameObject.FindGameObjectsWithTag("Music");
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if(allMusic.Length > 1)
        {
            switch (sceneIndex)
            {
                //if is title screen
                case 0:
                    DestroyMusic(allMusic, metalica);
                    DontDestroyOnLoad(this.gameObject);
                    break;
                //if is main scene
                case 3:
                    DestroyMusic(allMusic, raw);
                    break;
                //if is copless or cutscene
                default:
                    Destroy(this.gameObject);
                    break;
            }
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void DestroyMusic(GameObject[] allMusic, AudioClip clip)
    {
        foreach (GameObject music in allMusic)
        {
            if (music.GetComponent<AudioSource>().clip == clip)
            {
                Destroy(music);
            }
        }
    }
}
