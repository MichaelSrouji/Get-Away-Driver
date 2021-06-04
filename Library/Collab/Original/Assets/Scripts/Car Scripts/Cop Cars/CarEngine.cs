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
   public float playerSensorLength = 100f;
   public Vector3 frontSensorPosition = new Vector3(0f, 0.5f, 0.5f);
   public float frontSideSensorPosition = 0.8f;
   public float frontSensorAngle = 40f;
   public GameObject player;
   public float thresholdDistance = 7f;

   private List<GameObject> nodes;
   public GameObject currentNode;
   private GameObject newNode;
   private GameObject lastNode;
   private GameObject lastLastNode;

   private Vector3 lastKnownLocation;
   private bool visitedLoc = true;

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
      lastLastNode = lastNode;
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
      if (Vector3.Distance(transform.position, player.transform.position) < thresholdDistance)
      {
         status = "seePlayer";
      }
      else {
         if (currentNode.Equals(player)) FindClosestNode(transform.position);
         if (Vector3.Distance(transform.position, currentNode.transform.position) < thresholdDistance) CheckWaypointDistance();
      }

      switch(status)
      {
         case "seePlayer":
            Debug.Log("Seeing");
            currentNode = player;
            if (!pursuit) status = "trackingPlayer";
            //DriveIntoPlayer();
            //if break sight with player:
            //status = "trackingPlayer"
            break;
         case "trackingPlayer":
            UpdateTargetNode(player.transform.position);
            //LastKnownLocation();
            //reached last known point:
            //status = "SearchingPlayer"
            break;
         case "searchingPlayer":
            RandomTraversal();
            //RandomTraversal();
            //Randomly searching for the player
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
      if (playerHit.collider.gameObject.tag != "Player")
      {
         pursuit = false;
      }
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
      else if (avoiding && currentSpeed <= 0.5 && currentSpeed >= 0.2)
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
         status = "TrackingPlayer";
         //lastKnownLocation = player.transform.position;
         //visitedLoc = false;
         Debug.Log("Tracking");
      }
      /*else if((!pursuit) && (!visitedLoc))
      {
         status = "TrackingPlayer";
         if( (Vector3.Distance(transform.position, lastKnownLocation) < thresholdDistance))
         {
            visitedLoc = true;
         }
         Debug.Log("def");
      }*/
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
      int numAdj = currentNode.GetComponent<StreetNode>().adjacentIntersections.Length;
      float minDist = Vector3.Distance(currentNode.GetComponent<StreetNode>().adjacentIntersections[0].transform.position, player.transform.position);
      newNode = currentNode.GetComponent<StreetNode>().adjacentIntersections[0];
      foreach (GameObject node in currentNode.GetComponent<StreetNode>().adjacentIntersections) 
      {
         if(Vector3.Distance(node.transform.position,targetLocation) < minDist) 
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
      currentNode = newNode;
      lastLastNode = lastNode;
      lastNode = currentNode;

   }
}
