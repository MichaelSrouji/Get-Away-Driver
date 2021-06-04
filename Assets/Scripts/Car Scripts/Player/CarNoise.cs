using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNoise : MonoBehaviour
{
    private AudioSource carAudio;
    private Rigidbody rb;

    public float maxVolume = 5f;
    public float volumeLerpRate;
    public float pitchLerpRate = .001f;
    public float maxPitch;

    // Start is called before the first frame update
    void Start()
    {
        carAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        carAudio.pitch = Mathf.Lerp(carAudio.pitch, Mathf.Clamp(Mathf.Round(rb.velocity.magnitude/20 * 100f) / 100f + 1f, 0, maxPitch), 
                                                    Time.deltaTime * pitchLerpRate);
        //if driving forward or backwards
        if (Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
        {
            //if audio is not playing
            if(!carAudio.isPlaying)
            {
                Debug.Log("Turn on car rev");
                carAudio.Play();
            }
            carAudio.volume = Mathf.Lerp(carAudio.volume, //this value (a)
                                         Mathf.Clamp(Mathf.Abs(rb.velocity.magnitude) * .5f / 20f, 0, maxVolume), //will approach this value (b)
                                         Time.deltaTime * volumeLerpRate); // in this ammount of time (t)
        }
        //if not driving
        else
        {
            if(carAudio.volume == 0 && carAudio.isPlaying)
            {
                Debug.Log("Turn on car rev");
                carAudio.Stop();
            }
            else if(carAudio.volume != 0)
            {
                carAudio.volume = Mathf.Lerp(carAudio.volume, 0, volumeLerpRate * Time.deltaTime);
            }
        }
    }
}
