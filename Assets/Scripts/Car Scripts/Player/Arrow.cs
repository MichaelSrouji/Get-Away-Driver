using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
   public GameObject player;
   public GameObject CopCarManager;
   public Vector3 abovePlayer = new Vector3(0f, 1.5f, 0f);
   public float arrowY = 20f;

   void Start()
   {
      player = GameObject.FindWithTag("Player");
      CopCarManager = GameObject.FindWithTag("CopCarManager");
   }

   void Update()
   {
      transform.position = player.transform.position + abovePlayer;
      transform.LookAt(new Vector3(CopCarManager.transform.position.x, CopCarManager.transform.position.y, CopCarManager.transform.position.z));
   }
}
