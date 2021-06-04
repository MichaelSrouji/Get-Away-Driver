using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceScript : MonoBehaviour
{
    public float force = 0;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 upwards = new Vector3(0, force, 0);



        GetComponent<Rigidbody>().AddForce(upwards, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
