using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopCarManager : MonoBehaviour
{
   //Cop Car Variables
   private GameObject copCarPrefab;
   private GameObject[] copCars;
   public int numCopsInPursuit;

   //Node Path Variables
   public GameObject nodeParent;

   //Spawning Variables
   public int maxCopCars = 10;


   // Start is called before the first frame update
   void Start()
   {
      //make sure no two cop cars spawn at same location
      bool[] takenIndexes = new bool[nodeParent.transform.childCount];

      //spawn copCars
      copCars = new GameObject[maxCopCars];
      numCopsInPursuit = maxCopCars;

      for(int i = 0; i < maxCopCars; i++)
      {
         //choose random intersection to spawn a cop at
         int randomIndex = Random.Range(0, nodeParent.transform.childCount - 1);
         int loopCheck = 0;

         //make sure no cops are at an intersection
         while(takenIndexes[randomIndex] && loopCheck < 1000)
         {
            loopCheck++;
            randomIndex = Random.Range(0, nodeParent.transform.childCount - 1);
         }

         //sends message if looped too many times
         if(loopCheck >= 1000)
         {
            Debug.LogError("too many cop car loops");
         }
         else
         {
            copCars[i] = Instantiate(copCarPrefab,
                  nodeParent.transform.GetChild(randomIndex).transform.TransformPoint(Vector3.zero),
                  copCarPrefab.transform.rotation);
            copCars[i].copCarManager = this;
            copCars[i].currentNode = nodeParent.transform.GetChild(randomIndex);
            takenIndexes[randomIndex] = true;
         }
      }

   }


   private void OnTriggerEnter(Collider other)
   {
      if(other.gameObject.tag == "Player" && numCopsInPursuit <= 0)
      {
         //play win cutscene
      }
   }

   public void FindPlayer()
   {
      numCopsInPursuit++;
      // //turn arrow off
      // if( arrow on){
      //     turn arrow off;
      // }
   }

   public void LosePlayer()
   {
      numCopsInPursuit--;
      if(numCopsInPursuit == 0){
         //arrow on
      }
      //if NumCopsInPursuit == 0, turn on arrow
   }
}
