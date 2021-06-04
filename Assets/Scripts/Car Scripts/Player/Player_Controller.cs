using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    /*private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float speed = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }*/

    // Copied from https://stackoverflow.com/questions/54832462/car-movement-in-unity
    //public Rigidbody rb;
    //public Transform car;
    //public float speed = 17;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public float maxMotorTorque = 80f;
    public float currentSpeed;
    public float maxSpeed = 100f;


    Vector3 rotationRight = new Vector3(0, 115, 0);
    Vector3 rotationLeft = new Vector3(0, -115, 0);

    Vector3 forward = new Vector3(0, 0, 1);
    Vector3 backward = new Vector3(0, 0, -1);

    void FixedUpdate()
    {
        if (Input.GetKey("w"))
        {
            Drive();
        }
        if (Input.GetKey("s"))
        {
            Drive();
        }

        if (Input.GetKey("d"))
        {
            ApplySteer();
        }

        if (Input.GetKey("a"))
        {
            ApplySteer();
        }

    }
    private void ApplySteer()
    {
        Vector3 relativeVector = rotationRight;
        float newSteer = (relativeVector.y / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed)
        {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }
}
