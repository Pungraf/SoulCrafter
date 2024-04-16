using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public enum BehaviourState
    {
        Idle,
        Eating
    }

    public enum MovementState
    {
        Standing,
        Walking
    }

    public MovementState movementState = MovementState.Standing;
    public BehaviourState behaviourState = BehaviourState.Idle;

    private float behaviurCounter;

    private bool destinationReached;
    private NavMeshAgent navMeshAgent;
    private Unit unit;

    [SerializeField] Transform unitTransform;

    
    [SerializeField] private float walkRadius = 20f;
    [SerializeField] private float idleTimeSpeed = 20f;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();
        destinationReached = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(true)
        {
            IdleBehaviour();
        }
    }

    private void IdleBehaviour()
    {
        if(destinationReached && behaviurCounter <= 0)
        {
            FindeActivity();
        }

        if (!navMeshAgent.pathPending && movementState == MovementState.Walking)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    if(behaviourState == BehaviourState.Eating)
                    {
                        destinationReached = true;
                        Collider[] hitColliders = Physics.OverlapSphere(unit.transform.position, unit.InteractionRadius);
                        foreach (var hitCollider in hitColliders)
                        {
                            if (hitCollider.GetComponent<Food>() != null)
                            {
                                hitCollider.GetComponent<Food>().Eat(unit);
                                break;
                            }
                        }
                    }
                    else
                    {
                        FindeActivity();
                    }
                }
            }
        }
    }

    private void FindeActivity()
    {
        Collider[] hitColliders = Physics.OverlapSphere(unit.transform.position, unit.SenseRadius);

        unit.Hungry += 10;

        if(unit.Hungry > 50)
        {
            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.GetComponent<Food>() != null)
                {
                    movementState = MovementState.Walking;
                    Vector3 finalPosition = hitCollider.transform.position;
                    navMeshAgent.SetDestination(finalPosition);
                    behaviourState = BehaviourState.Eating;
                    destinationReached = false;
                    break;
                }
            }
            IdleBehaviour();
        }
        else
        {
            movementState = MovementState.Walking;
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
            Vector3 finalPosition = hit.position;
            navMeshAgent.SetDestination(finalPosition);
            destinationReached = false;
        }
    }
}
