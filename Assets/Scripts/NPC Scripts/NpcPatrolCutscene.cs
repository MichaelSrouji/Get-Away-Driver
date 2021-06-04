using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcPatrolCutscene : MonoBehaviour
{
    //Components
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    //Wondering Variables
    public float targetDistance = 1f;
    public Vector3 targetLocation;
    public Vector3 spawn;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshHit hit;

        //set up components
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        spawn = transform.position;
        
        NavMesh.SamplePosition(targetLocation, out hit, 5, NavMesh.AllAreas);
        targetLocation = hit.position;
        navMeshAgent.SetDestination(targetLocation);

        NavMesh.SamplePosition(spawn, out hit, 5, NavMesh.AllAreas);
        spawn = hit.position;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", Mathf.Abs(navMeshAgent.velocity.magnitude) / navMeshAgent.speed);
        //if NPC reaches wonder destination or cannot reach their destination
        if (Vector3.Distance(transform.position, targetLocation) < targetDistance)
        {
            navMeshAgent.Warp(spawn);
        }
    }
}
