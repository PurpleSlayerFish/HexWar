using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentController : MonoBehaviour
{

    [SerializeField]
    Transform target;

    private NavMeshAgent agent;
    
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        agent.SetDestination(target.position);
    }
}
