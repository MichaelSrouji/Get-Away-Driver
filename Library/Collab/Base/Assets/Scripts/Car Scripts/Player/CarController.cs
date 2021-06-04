using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    public float motorForce;
    public float breakForce;
    public float maxSteerAngle;

    public Rigidbody car;
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheeTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    private Vector3 spawn;
    private Quaternion spawnRot;

    public Vector3 centerOfMass;
    
    //Health Variables
    public int health = 100;
    public float speed;
    private bool isDead;
    public HealthScript healthBar;


    private void Start()
    {
        spawn = transform.position;
        spawnRot = transform.rotation;
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthScript>();
    }

    private void Update()
    {
        if (!isDead) {
            speed = car.velocity.magnitude;

            GetInput();
            HandleSteering();
            HandleMotor();
            UpdateWheels();
            if(Input.GetKeyDown(KeyCode.R))
            {
                health = 100;
                transform.position = spawn;
                transform.rotation = spawnRot;
            }
        }
    }

    private void FixedUpdate()
    {
    }

    private void GetInput()
    {
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        //frontLeftWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorForce;
        //frontRightWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorForce;
        rearLeftWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorForce;
        rearRightWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }

    private void ApplyBreaking()
    {
        //frontRightWheelCollider.brakeTorque = currentbreakForce;
        //frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;       
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void Explode() {
        ParticleSystem exp = GetComponent<ParticleSystem>();
        exp.Play();
    }

    IEnumerator waitTime() {
        isDead = true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }

    private void OnCollisionExit(Collision col) {
        
    }

    private void OnCollisionEnter(Collision col) {

        if ((col.gameObject.tag != "Terrain") && (col.gameObject.tag != "NPC"))
        {
            if (speed > 25) {
                healthBar.DecreaseHealth(health / 100f, .5f);
                health -= 50;
            }
            else if (speed > 20) {
                healthBar.DecreaseHealth(health / 100f, .3f);
                health -= 30;
            }
            else if (speed > 15) {
                healthBar.DecreaseHealth(health / 100f, .2f);
                health -= 20;
            }
            else if (speed > 10) {
                healthBar.DecreaseHealth(health / 100f, .1f);
                health -= 10;
            }
            else if (speed > 5) {
                healthBar.DecreaseHealth(health / 100f, .05f);
                health -= 5;
            }
            else if (speed > 1) {
                healthBar.DecreaseHealth(health / 100f, .01f);
                health -= 1;
            }
            Debug.Log(health);

            if (health <= 0) {
                if (isDead == false)
                {
                    Explode();
                    StartCoroutine(waitTime());
                }
            }   
        }
    }
}