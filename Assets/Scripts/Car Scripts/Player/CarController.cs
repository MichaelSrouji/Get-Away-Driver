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

    //particle systems
    public ParticleSystem fireParticle;
    public ParticleSystem firesparks;
    public ParticleSystem smoke;
    public ParticleSystem explosion;


    public AudioSource explosionNoise;

    public GameObject frontOfCar;

    private void Start()
    {
        spawn = transform.position;
        spawnRot = transform.rotation;
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;


        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthScript>();
    }

    private void Update()
    {
        if (!isDead)
        {
            speed = car.velocity.magnitude;

            GetInput();
            HandleSteering();
            HandleMotor();
            UpdateWheels();
        }
    }

    private void FixedUpdate()
    {
        ParticleSystem.MainModule main = smoke.main;
        Color temp = new Color32(255, 255, 255, (byte)(.6 * (255 - health / .3921568627)));
        main.startColor = temp;

    }

    private void GetInput()
    {
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorForce;
        rearRightWheelCollider.motorTorque = Input.GetAxis("Vertical") * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();

        if (Input.GetAxis("Vertical") == 0)
        {
            car.drag = 1;
        }
        else
        {
            car.drag = .05f;
        }
    }

    private void ApplyBreaking()
    {
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

    private void Explode()
    {
        explosionNoise.Play();
        explosion.Play();
        fireParticle.Play();
        firesparks.Play();
    }

    IEnumerator waitTime()
    {
        isDead = true;
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter(Collision col)
    {

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "Copless Scene")
        {
            if ((col.gameObject.tag != "Terrain") && (col.gameObject.tag != "NPC"))
            {
                if (speed > 25)
                {
                    healthBar.DecreaseHealth(health / 100f, .5f);
                    health -= 50;
                }
                else if (speed > 20)
                {
                    healthBar.DecreaseHealth(health / 100f, .3f);
                    health -= 30;
                }
                else if (speed > 15)
                {
                    healthBar.DecreaseHealth(health / 100f, .2f);
                    health -= 20;
                }
                else if (speed > 10)
                {
                    healthBar.DecreaseHealth(health / 100f, .1f);
                    health -= 10;
                }
                else if (speed > 5)
                {
                    healthBar.DecreaseHealth(health / 100f, .05f);
                    health -= 5;
                }
                else if (speed > 1)
                {
                    healthBar.DecreaseHealth(health / 100f, .01f);
                    health -= 1;
                }
                Debug.Log("health: " + health);

                if (health <= 0)
                {
                    if (isDead == false)
                    {
                        Explode();
                        StartCoroutine(waitTime());
                    }
                }
            }
        }
    }
}