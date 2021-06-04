using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityScroll : MonoBehaviour
{
    public float citySpeed = 10f;
    public float clamp = 10;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > clamp)
        {
            transform.position = Vector3.zero;
        }
        else
        {
            transform.position += Vector3.forward * citySpeed * Time.deltaTime;
        }
    }
}
