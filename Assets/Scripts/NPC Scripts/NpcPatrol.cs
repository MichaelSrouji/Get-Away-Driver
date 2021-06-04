using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcPatrol : MonoBehaviour
{
    //Components
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private Rigidbody rb;

    //Wondering Variables
    public float wonderRange = 20f;
    public float targetDistance = 1f;
    private Vector3 targetLocation;

    //Dying Variables
    private bool isDead = false;
    public float upwardsForce = 5f;
    public float minForce = 1f;
    public GameObject hips;
    private Rigidbody hipsRb;

    //Spawning Variables
    private GameObject player;
    public NpcManager npcManager;
    public float MaxDistanceFrontPlayer = 50;
    public float MaxDistanceBehindPlayer = 50;
    public float degreesFromPlayer = 180;

    //Scream Variables
    public float screamPitch = 1f;


    // Start is called before the first frame update
    void Start()
    {
        //disable all rigid bodies
        RagDollEnabler(false);

        //set up components
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        SetWonderDestination();

        //set hips for dying force
        hipsRb = hips.GetComponent<Rigidbody>();

        //find player
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Could not find player, deleting self");
            npcManager.DespawnNPC();
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //set walk speed if not dead
        if(!isDead)
        {
            anim.SetFloat("Speed", Mathf.Abs(navMeshAgent.velocity.magnitude) / navMeshAgent.speed);

            //if NPC reaches wonder destination or cannot reach their destination
            if (Vector3.Distance(transform.position, targetLocation) < targetDistance || navMeshAgent.velocity.magnitude < .1)
            {
                SetWonderDestination();
            }
        }
        //delete NPC
        CheckDespawn();

    }

    private void CheckDespawn()
    {
        if (player != null)
        {
            Vector3 hipsLocation = hips.transform.position;
            Vector3 toPlayer = player.transform.position - hipsLocation;
            Vector3 toNpc = hipsLocation - player.transform.position;
            toNpc.Normalize();
            toPlayer.Normalize();

            //Debug.DrawRay(player.transform.position, toNpc * 5, Color.green);
            //Debug.DrawRay(player.transform.position, player.transform.forward * 5, Color.green);

            //if NPC is in front of player
            if (Vector3.Angle(player.transform.forward, toNpc) < degreesFromPlayer)
            {
                if (isDead)
                {
                    Debug.DrawRay(hipsLocation, toPlayer * Vector3.Distance(hipsLocation, player.transform.position), Color.green);
                }
                else
                {
                    Debug.DrawRay(hipsLocation, toPlayer * Vector3.Distance(hipsLocation, player.transform.position), Color.blue);
                }
                

                //if its outside of the range of the player
                if (Vector3.Distance(hipsLocation, player.transform.position) > MaxDistanceFrontPlayer)
                {
                    npcManager.DespawnNPC();
                    Destroy(this.gameObject);
                }
            }
            //if NPC is behind player
            else
            {
                if (isDead)
                {
                    Debug.DrawRay(hipsLocation, toPlayer * Vector3.Distance(hipsLocation, player.transform.position), Color.red);
                }
                else
                {
                    Debug.DrawRay(hipsLocation, toPlayer * Vector3.Distance(hipsLocation, player.transform.position), Color.yellow);
                }

                //if its outside of the range of the player
                if (Vector3.Distance(hipsLocation, player.transform.position) > MaxDistanceBehindPlayer)
                {
                    npcManager.DespawnNPC();
                    Destroy(this.gameObject);
                }
            }
        }
    }


    //this method finds a new wonderpoint for the NPC
    private void SetWonderDestination()
    {
        Vector3 localRandom = new Vector3(transform.position.x + Random.Range(-wonderRange, wonderRange),
                                          .2f,
                                          transform.position.z + Random.Range(-wonderRange, wonderRange));
        NavMeshHit hit;
        NavMesh.SamplePosition(localRandom, out hit, wonderRange, NavMesh.AllAreas);
        targetLocation = hit.position;
        navMeshAgent.SetDestination(targetLocation);
    }

    //this method sets up the ragdoll
    private void RagDollDeath()
    {
        Rigidbody[] children = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody child in children)
        {
            child.isKinematic = false;
        }
    }

    //this method sees if the NPC was hit by the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Cop")
        {
            GameObject player = other.gameObject;
            Vector3 directionVector = transform.position - player.transform.position;
            Vector3 forceVector;
            float playerSpeed = Mathf.Abs(player.GetComponentInParent<Rigidbody>().velocity.magnitude);
            AudioSource scream = GetComponentInChildren<AudioSource>();

            if(!isDead)
            {
                //Play NPC scream
                scream.pitch = screamPitch;
                scream.Play();
            }

            //NPC Dies
            isDead = true;
            anim.enabled = false;
            navMeshAgent.enabled = false;

            //Rigid Body (ragdoll later on (?))
            transform.position += new Vector3(0, .1f, 0);

            //force NPC away
            directionVector.Normalize();
            directionVector += Vector3.up * .2f;
            directionVector.Normalize();
            forceVector = directionVector * playerSpeed * minForce;

            RagDollEnabler(true);
            hipsRb.AddForce(forceVector, ForceMode.Impulse);
        }
    }

    //this helper function disables all the rigid bodies for the start method
    private void RagDollEnabler(bool isActive)
    {
        //disable all rigid bodies 
        Rigidbody[] children = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Rigidbody child in children)
        {
            child.isKinematic = !isActive;
        }
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject != this.gameObject)
            {
                collider.enabled = isActive;
            }
        }
    }
}
