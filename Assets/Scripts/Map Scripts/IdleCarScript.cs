using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleCarScript : MonoBehaviour
{
    //Timing Variables
    private float timeToReset = 10f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
            StopAllCoroutines();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(TurnOnKinematic());
        }
    }

    IEnumerator TurnOnKinematic()
    {
        Debug.Log("in coroutine");
        yield return new WaitForSeconds(timeToReset);
        rb.isKinematic = true;
        Debug.Log("finished coroutine");
    }
}
