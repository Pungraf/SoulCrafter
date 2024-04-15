using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walking
    }

    public State state = State.Idle;

    private float behaviurCounter;
    private float currentYPosition;
    private bool destinationReached;
    private NavMeshAgent navMeshAgent;

    [SerializeField] Transform unitTransform;

    [SerializeField] private float floatingSpeed = 1f;
    [SerializeField] private float floatingAmp = 0.5f;
    [SerializeField] private float walkRadius = 20f;
    [SerializeField] private float idleTimeSpeed = 20f;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentYPosition = unitTransform.position.y;
        destinationReached = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (destinationReached && state == State.Idle)
        {
            behaviurCounter -= Time.deltaTime * idleTimeSpeed;
            if(behaviurCounter < 0 )
            {
                state = State.Walking;
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                Vector3 finalPosition = hit.position;
                navMeshAgent.SetDestination(finalPosition);
            }
        }
        Vector3 newPosition = new Vector3(unitTransform.position.x, currentYPosition + floatingAmp * Mathf.Sin(floatingSpeed * Time.time), unitTransform.position.z);
        unitTransform.position = newPosition;

        if (!navMeshAgent.pathPending && state == State.Walking)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    destinationReached = true;
                    behaviurCounter = 100f;
                    state = State.Idle;
                }
            }
        }
    }
}
