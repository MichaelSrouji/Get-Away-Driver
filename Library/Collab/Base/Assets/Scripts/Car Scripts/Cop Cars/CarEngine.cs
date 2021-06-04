using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
   public Transform path;
   public float maxSteerAngle = 45f;
   public WheelCollider wheelFL;
   public WheelCollider wheelFR;
   public WheelCollider wheelRL;
   public WheelCollider wheelRR;
   public float maxMotorTorque = 250f;
   public float maxBrakeTorque = 300f;
   public float currentSpeed;
   public float maxSpeed = 6f;
   public Rigidbody car;

   [Header("Sensors")]
      public float sensorLength = 5f;
   public float forwardSensorLength = 5f;
   public float backwardSensorLength = 8f;
   private float playerSensorLength = Mathf.Infinity;
   public Vector3 frontSensorPosition = new Vector3(0f, 0.5f, 0.5f);
   public float frontSideSensorPosition = 0.8f;
   public float frontSensorAngle = 30f;
   public GameObject player;
   public float thresholdDistance = 7f;

   private List<GameObject> nodes;
   public GameObject currentNode;
   private GameObject newNode;
   private GameObject lastNode;
   private Vector3 lastLoc;

   private Vector3 lastKnownLocation;

   public bool avoiding = false;
   public bool pursuit = false;
   private float targetSteerAngle = 0f;

   private string status = "trackingPlayer";

   // Layermask for player detection
   public int playerMask;

   void Start()
   {
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
      else if (currentNode.Equals(player)) FindClosestNode(transform.position);
      else if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) {
         CheckWaypointDistance();
      }

      switch(status)
      {
         case "seePlayer":
            Debug.Log("Seeing");
            lastLoc = transform.position;
            currentNode = player;
            if (!pursuit) status = "trackingPlayer";
            break;
         case "trackingPlayer":
            if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) {
               if (Vector3.Distance(transform.position, lastLoc) > thresholdDistance * 2) UpdateTargetNode(lastLoc);
               else UpdateTargetNode(player.transform.position);
            }
            break;
         case "searchingPlayer":
            lastLoc = transform.position;
            if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) RandomTraversal();
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
      float avoidMultiplier = 0f;
      currentSpeed = car.velocity.magnitude;
      avoiding = false;

      // Front Right Sensor
      sensorStartPos += transform.right * frontSideSensorPosition;
      if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 1f;
         }
         else if (hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 0f;
         }
      }

      // Front Right Angle Sensor
      else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 0.5f;
         }
         else if (hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 0.5f;
         }
      }

      // Front Left Sensor
      sensorStartPos -= 2 * transform.right * frontSideSensorPosition;
      if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 1f;
         }
         else if (hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 0f;
         }
      }

      // Front Left Angle Sensor
      else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
      {
         if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 0.5f;
         }
         else if (hit.collider.CompareTag("Player"))
         {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 0.5f;
         }
      }

      // Front Center Sensor
      if (avoidMultiplier == 0)
      {
         if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
         {
            if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("Player"))
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
            else if (hit.collider.CompareTag("Player"))
            {
               Debug.DrawLine(sensorStartPos, hit.point);
               avoiding = true;
               if (hit.normal.x < 0)
               {
                  avoidMultiplier = 0f;
               }
               else
               {
                  avoidMultiplier = 0f;
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
      if (gameObject.tag == "Player") return;
      RaycastHit playerHit;
      Vector3 sensorStartPos = transform.position;
      sensorStartPos += transform.forward * frontSensorPosition.z;
      sensorStartPos += transform.up * frontSensorPosition.y;
      var direction = (player.transform.position - transform.position).normalized;
      Physics.Raycast(sensorStartPos, direction, out playerHit, playerSensorLength, playerMask);
      if (playerHit.collider.gameObject.tag != "Player") pursuit = false;
      else pursuit = true;
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

      if (currentSpeed < maxSpeed && !avoiding)
      {
         sensorLength = forwardSensorLength;
         wheelFL.motorTorque = maxMotorTorque;
         wheelFR.motorTorque = maxMotorTorque;
      }
      else if (avoiding && currentSpeed >= maxSpeed / 2)
      {
         wheelFL.motorTorque = 0;
         wheelFR.motorTorque = 0;
      }
      else if (avoiding && currentSpeed <= 0.4 && currentSpeed >= 0.1)
      {
         sensorLength = backwardSensorLength;
         wheelFL.motorTorque = -maxMotorTorque * 12;
         wheelFR.motorTorque = -maxMotorTorque * 12;
      }
      else
      {
         sensorLength = forwardSensorLength;
         wheelFL.motorTorque = maxMotorTorque;
         wheelFR.motorTorque = maxMotorTorque;
      }
   }

   private void CheckWaypointDistance()
   {
      if (pursuit)
      {
         status = "trackingPlayer";
         lastLoc = player.transform.position;
         Debug.Log("Tracking");
      }
      else if (Vector3.Distance(transform.position, lastLoc) > thresholdDistance * 2)
      {
         status = "trackingPlayer";
         Debug.Log("LastLoc");
      }
      else
      {
         Debug.Log("Random");
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
      Debug.Log(currentNode.name);
   }

   private void FindClosestNode(Vector3 targetLocation)
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
}
