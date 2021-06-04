using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopWheelTurning : MonoBehaviour
{
   public WheelCollider frontLeftWheelCollider;
   public WheelCollider frontRightWheelCollider;
   public WheelCollider rearLeftWheelCollider;
   public WheelCollider rearRightWheelCollider;

   public Transform frontLeftWheelTransform;
   public Transform frontRightWheeTransform;
   public Transform rearLeftWheelTransform;
   public Transform rearRightWheelTransform;

   // Update is called once per frame
   void Update()
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
}
