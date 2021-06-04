using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    public int zRotLimit = 20;
    public float rotateForce = 100f;
    public float localFlipForce = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.cyan);
        RollCar();
    }

    private void RollCar()
    {
        float carRot = Vector3.Angle(transform.up, Vector3.up);
        if (carRot > zRotLimit)
        {
            Debug.DrawLine(transform.position, transform.position + transform.up * 4, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + -transform.right * 4, Color.magenta);
            localFlipForce += rotateForce * Mathf.Deg2Rad * carRot;
            rb.AddTorque(transform.forward * localFlipForce * Mathf.Deg2Rad * Vector3.Angle(transform.up, Vector3.up), ForceMode.Force);
        }
        else if (carRot < -zRotLimit)
        {
            Debug.DrawLine(transform.position, transform.position + transform.up * 4, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + -transform.right * 4, Color.magenta);
            localFlipForce -= rotateForce * Mathf.Deg2Rad * carRot;
            rb.AddTorque(transform.forward * localFlipForce * Mathf.Deg2Rad * Vector3.Angle(transform.up, Vector3.up), ForceMode.Force);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 4, Color.green);
            localFlipForce = 0;
        }
    }
}
