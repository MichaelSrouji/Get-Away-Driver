using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
   // Wheels for adding torque
   public Rigidbody car;
   public WheelCollider wheelFL;
   public WheelCollider wheelFR;
   public WheelCollider wheelRL;
   public WheelCollider wheelRR;

   // Turning and speed values
   public float maxMotorTorque = 600f;
   public float maxBrakeTorque = 1200f;
   public float currentSpeed;
   public float maxSpeed = 5f;
   public float maxSteerAngle = 45f;

   // For everything having to do with sensors
   [Header("Sensors")]
   public float sensorLength = 4f;
   public float forwardSensorLength = 4f;
   public float backwardSensorLength = 4f;
   private float playerSensorLength = Mathf.Infinity;
   public Vector3 frontSensorPosition = new Vector3(0f, 0.5f, 0.5f);
   public float frontSideSensorPosition = 0.8f;
   public float frontSensorAngle = 30f;
   public float thresholdDistance = 5f;

   // For keeping track of node pathing
   public Transform path;
   private List<GameObject> nodes;
   public GameObject currentNode;
   private GameObject newNode;
   private GameObject lastNode;

   // For tracking down the player and greedy pathing
   public CopCarManager copManager;
   public GameObject player;
   private Vector3 lastLoc;
   private bool lastLocBool = false;
   private Vector3 lastKnownLocation;

   // For avoiding obstacles and ramming into player
   public bool avoiding = false;
   public bool pursuit = false;
   private float targetSteerAngle = 0f;

   // Switch cases
   private string status = "trackingPlayer";
   private string driveMode = "forward";

   // Coroutine
   private IEnumerator coroutine;
   public float waitTime = 10.0f;
   private bool inCo = true;
   private bool outCo = false;

   // Layermask for player detection
   public int playerMask;

   void Start()
   {
      copManager = GameObject.FindWithTag("CopCarManager").GetComponent<CopCarManager>();
      path = GameObject.FindWithTag("path").transform;
      Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
      nodes = new List<GameObject>();
      player = GameObject.FindWithTag("Player");
      playerMask = ~LayerMask.GetMask("CopDetection");
      // InvokeRepeating("PlayerSensor", 0.0f, 2.0f);

      for (int i = 0; i < pathTransforms.Length; i++)
      {
         if (pathTransforms[i] != path.transform)
         {
            nodes.Add(pathTransforms[i].gameObject);
         }
      }
      currentNode = FindClosestNode();
      lastNode = currentNode;
   }

   private void FixedUpdate()
   {
      Sensors();
      ApplySteer();
      Drive();
      LerpToSteerAngle();
   }

   private void Update()
   {
      PlayerSensor();
      if (Vector3.Distance(transform.position, player.transform.position) < thresholdDistance * 2)
      {
         status = "seePlayer";
      }
      else if (currentNode.Equals(player)) currentNode = FindClosestNode();
      else if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) {
         CheckWaypointDistance();
      }

      switch(status)
      {
         case "seePlayer":
            // Debug.Log("Seeing");
            currentNode = player;
            if (!pursuit) {
               if (inCo) {
                  coroutine = ChasePlayer(waitTime);
                  StartCoroutine(coroutine);
                  // Debug.Log("Coroutine started");
               }
               if (outCo) {
                  outCo = false;
                  inCo = true;
                  // Debug.Log("Coroutine ended");
                  status = "trackingPlayer";
               }
            }
            break;
         case "trackingPlayer":
            if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) {
               UpdateTargetNode(player.transform.position);
            }
            break;
         case "searchingPlayer":
            if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) {
               if (lastLocBool) {
                  UpdateTargetNode(lastLoc);
                  if (Vector3.Distance(transform.position, lastLoc) < thresholdDistance * 2) lastLocBool = false;
               }
               else RandomTraversal();
            }
            break;
         default:
            RandomTraversal();
            break;
      }
   }

   private void Sensors()
   {
      RaycastHit hit;
      Vector3 sensorStartPos = transform.position;
      sensorStartPos += transform.forward * frontSensorPosition.z;
      sensorStartPos += transform.up * frontSensorPosition.y * 2;
      currentSpeed = car.velocity.magnitude;
      float avoidMultiplier = 0f;
      avoiding = false;

      // Front Right Sensor
      sensorStartPos += transform.right * frontSideSensorPosition;
      if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("NPC"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 1f;
         }
      }

      // Front Right Angle Sensor
      else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("NPC"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 0.5f;
         }
      }

      // Front Left Sensor
      sensorStartPos -= 2 * transform.right * frontSideSensorPosition;
      if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("NPC"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 1f;
         }
      }

      // Front Left Angle Sensor
      else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("NPC"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 0.5f;
         }
      }

      // Front Center Sensor
      if (avoidMultiplier == 0)
      {
         if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
         {
            if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("NPC"))
            {
               Debug.DrawLine(sensorStartPos, hit.point);
               avoiding = true;
               if (hit.normal.x < 0)
               {
                  avoidMultiplier = -1f;
               }
               else
               {
                  avoidMultiplier = 1f;
               }
            }
         }
      }

      if (avoiding)
      {
         targetSteerAngle = maxSteerAngle * avoidMultiplier;
      }
   }

   private void PlayerSensor()
   {
      RaycastHit playerHit;
      Vector3 sensorStartPos = transform.position;
      sensorStartPos += transform.forward * frontSensorPosition.z;
      sensorStartPos += transform.up * frontSensorPosition.y;
      var direction = (player.transform.position - transform.position).normalized;
      Physics.Raycast(sensorStartPos, direction, out playerHit, playerSensorLength, playerMask);
      if (playerHit.collider.gameObject.tag != "Player")
      {
         if (pursuit == true) {
            copManager.LosePlayer();
            pursuit = false;
         }
      }
      else 
      {
         if (pursuit == false) {
            copManager.FindPlayer();
            pursuit = true;
         }
      }
      Debug.DrawLine(transform.position, playerHit.point);
   }

   private void ApplySteer()
   {
      if (avoiding) return;
      Vector3 relativeVector = transform.InverseTransformPoint(currentNode.transform.position);
      float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
      targetSteerAngle = newSteer;
   }

   private void Drive()
   {
      currentSpeed = car.velocity.magnitude;
      var localVel = transform.InverseTransformDirection(car.velocity);

      if (currentSpeed < maxSpeed && !avoiding)
      {
         driveMode = "forward";
      }
      else if (currentNode != player && (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance * 3))
      {
         driveMode = "braking";
         if (localVel.z < 2) driveMode = "forward";
         // Debug.Log("Braking");
      }
      else if (avoiding && wheelFL.motorTorque < 0 && currentSpeed <= 0.3)
      {
         if (driveMode != "forward") driveMode = "forward";
         else driveMode = "backward";
      }
      else if (avoiding && currentSpeed <= 0.5)
      {
         if (driveMode != "backward") driveMode = "backward";
         else driveMode = "forward";
      }

      switch(driveMode)
      {
         case "forward":
            sensorLength = forwardSensorLength;
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
            break;
         case "braking":
            wheelFL.motorTorque = -maxBrakeTorque;
            wheelFR.motorTorque = -maxBrakeTorque;
            break;
         case "backward":
            sensorLength = backwardSensorLength;
            wheelFL.motorTorque = -maxMotorTorque * 4;
            wheelFR.motorTorque = -maxMotorTorque * 4;
            break;
         default:
            sensorLength = forwardSensorLength;
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
            break;
      }
   }

   private void CheckWaypointDistance()
   {
      if (pursuit)
      {
         status = "trackingPlayer";
         lastLoc = player.transform.position;
         lastLocBool = true;
      }
      else
      {
         status = "searchingPlayer";
      }
   }

   private void LerpToSteerAngle()
   {
      wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * 10);
      wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * 10);
   }

   private void UpdateTargetNode(Vector3 targetLocation) 
   {
      float minDist = Mathf.Infinity;
      foreach (GameObject node in currentNode.GetComponent<StreetNode>().adjacentIntersections) 
      {
         if(Vector3.Distance(node.transform.position, targetLocation) < minDist) 
         {
            newNode = node;
            minDist = Vector3.Distance(node.transform.position, targetLocation);
         }
      }
      currentNode = newNode;
      // Debug.Log(currentNode.name);
   }

   private GameObject FindClosestNode()
   {
      float minDist = Mathf.Infinity;
      foreach (GameObject node in nodes)
      {
         float checkDist = Vector3.Distance(node.transform.position, transform.position);
         if(checkDist < minDist) {
            currentNode = node;
            minDist = checkDist;
         }
      }
      return currentNode;
   }

   private void RandomTraversal()
   {
      int numAdj = currentNode.GetComponent<StreetNode>().adjacentIntersections.Length;
      newNode = currentNode.GetComponent<StreetNode>().adjacentIntersections[Random.Range(0, numAdj)];
      while (newNode.Equals(lastNode)) {
         newNode = currentNode.GetComponent<StreetNode>().adjacentIntersections[Random.Range(0, numAdj)];
      }
      lastNode = currentNode;
      currentNode = newNode;
   }

   IEnumerator ChasePlayer(float waitTime)
   {
      inCo = false;
      yield return new WaitForSeconds(waitTime);
      outCo = true;
   }
}
