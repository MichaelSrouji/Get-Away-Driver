using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CopCarManager : MonoBehaviour
{
    //Cop Car Variables
    public GameObject CopPrefab;
    private GameObject[] copCars;
    public GameObject arrow;
    public int numCopsInPursuit;

    //Node Path Variables
    public GameObject nodeParent;

    //Spawning Variables
    public int maxCopCars = 35; //
    public int numStartCops = 0;  //
    public int rand;
    public int max_gold_bars = 12;

    // Difficulty Variables
    public float difficultyPercent;  //

    // Script Variables
    public RobberSpeech robber;
    public CopCarDetectionBar copBar;

    //Win Condition Variables
    private bool hasWon = false;

    // Start is called before the first frame update
    void Start()
    { 
        difficultyPercent = ((float)DifficultyManager.goldBars)/max_gold_bars;
        numStartCops = (int)(maxCopCars * difficultyPercent); //

        //make sure no two cop cars spawn at same location
        var takenMap = new Dictionary<int, bool>(); // 

        //spawn copCars
        numCopsInPursuit = 0;

        /* Plan:
            have a map of all nodes: false  --> taken map
            in a loop from 0:numStartCops
                rand = random(0:numOfNodes) 
                while map[rand] != false
                    rand = random(0:startCops);
                spawn cop at rand node
        */

        // Instantiate takenMap //
        for(int i =0; i < (nodeParent.transform.childCount); i++){  //
            takenMap[i]=false;  //
        }

        /**/
        for (int i = 0; i < numStartCops; i++)
        {
            rand = Random.Range(0, nodeParent.transform.childCount);
            while(takenMap[rand])
            {
                rand = Random.Range(0, nodeParent.transform.childCount);
            }
            takenMap[rand] = true;
            spawn_cop(rand);
        }
        /**/
    }

    private void spawn_cop(int node)
    {
        Vector3 node_loc = nodeParent.transform.GetChild(node)  
            .transform.TransformPoint(Vector3.zero); 
        Instantiate(CopPrefab, node_loc, CopPrefab.transform.rotation);    
    }

    private void OnTriggerStay(Collider other)
    {
        if (!hasWon && other.gameObject.tag == "Player" && numCopsInPursuit <= 0)
        {
            hasWon = true;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void FindPlayer()
    {
        numCopsInPursuit++;
        copBar.NewCopCar();
        if (arrow.activeSelf)
        {
            arrow.SetActive(false);
        }
    }

    public void LosePlayer()
    {
        numCopsInPursuit--;
        copBar.LoseCopCar();
        if (numCopsInPursuit == 0)
        {
            arrow.SetActive(true);
        }
    }
}
