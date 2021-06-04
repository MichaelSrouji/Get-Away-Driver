using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSoundEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.time = Random.Range(0, audio.clip.length);
        audio.Play();
    }
}
