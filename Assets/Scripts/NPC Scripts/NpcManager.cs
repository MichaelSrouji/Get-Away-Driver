using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcManager : MonoBehaviour
{
    //NPC Variables
    private int numNpcs = 0;
    public int maxNpcs = 50;

    //Spawning Variables
    public float minDistanceFromPlayer = 30;
    public float maxDistanceFromPlayer = 40;
    public float AngleInFrontOfPlayer = 30;

    //Player Variables
    public GameObject player;

    //NPC Variables
    public GameObject[] npcs;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && npcs.Length > 0)
        {
            float spawnDist = 10f;
            while (numNpcs < maxNpcs)
            {
                GameObject currentNpc;
                NavMeshHit hit;
                Vector3 playerPos = player.transform.position;
                Vector3 playerRot = Quaternion.Euler(0, Random.Range(-AngleInFrontOfPlayer, AngleInFrontOfPlayer), 0) * player.transform.forward;
                Vector3 spawnPos = playerPos + playerRot * Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
                int npcIndex = Random.Range(0, npcs.Length);

                //Find Spawn Positions
                if(NavMesh.SamplePosition(spawnPos, out hit, spawnDist, NavMesh.AllAreas))
                {
                    //If hit position isn't an infinite position
                    currentNpc = Instantiate(npcs[npcIndex], hit.position, npcs[npcIndex].transform.rotation);
                    currentNpc.GetComponent<NpcPatrol>().npcManager = this;
                    spawnDist = 10f;

                    //incriment NPCs
                    numNpcs++;
                }
                else
                {
                    spawnDist += 10f;
                }
            }
        }
        
    }

    public void DespawnNPC()
    {
        numNpcs--;
    }
}
