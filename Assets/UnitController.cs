using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public enum BehaviourState
    {
        None,
        Idle,
        SearchingForFood,
        Eating,
        SearchingForDrink,
        Drinking,
        Wandering
    }


    [SerializeField] private BehaviourState currentBehaviourState = BehaviourState.None;


    public BehaviourState CurrentBehaviourState
    {
        get { return currentBehaviourState; }
        set { currentBehaviourState = value; }
    }

    private float behaviurCounter;
    private NavMeshAgent navMeshAgent;
    private Unit unit;
    private Transform targetedTransform;

    [SerializeField] Transform unitTransform;


    [SerializeField] private float walkRadius = 20f;
    [SerializeField] private float idleTime = 5f;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();

        CurrentBehaviourState = BehaviourState.None;
    }


    // Update is called once per frame
    void Update()
    {
        unit.Hungry += Time.deltaTime;
        unit.Thirst += Time.deltaTime;
        behaviurCounter -= Time.deltaTime;

        ExecuteBehaviour();
    }

    private void ExecuteBehaviour()
    {
        switch(CurrentBehaviourState)
        {
            case BehaviourState.None:
                FindeActivity();
                break;
            case BehaviourState.Idle:
                behaviurCounter -= Time.deltaTime;
                if(behaviurCounter <= 0)
                {
                    FindeActivity();
                }
                break;
            case BehaviourState.Wandering:
                BehaviourDestintaionReached();
                break;
            case BehaviourState.SearchingForFood:
                BehaviourDestintaionReached();
                break;
            case BehaviourState.Eating:
                if(targetedTransform != null)
                {
                    targetedTransform.TryGetComponent<Food>(out Food currnetFood);
                    if (currnetFood != null && unit.Hungry > 1)
                    {
                        currnetFood.Eat(unit);
                        break;
                    }
                }
                currentBehaviourState = BehaviourState.None;
                break;
            case BehaviourState.SearchingForDrink:
                BehaviourDestintaionReached();
                break;
            case BehaviourState.Drinking:
                if (targetedTransform != null)
                {
                    targetedTransform.TryGetComponent<Drink>(out Drink currnetDrink);
                    if (currnetDrink != null && unit.Thirst > 1)
                    {
                        currnetDrink.Drinking(unit);
                        break;
                    }
                }
                currentBehaviourState = BehaviourState.None;
                break;
        }
    }

    

    private void BehaviourDestintaionReached()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    FindeActivity();
                }
            }
        }
    }

    private void FindeActivity()
    {
        if (unit.Hungry > unit.HungryTreshold)
        {
            Collider[] inSenseRadius = Physics.OverlapSphere(unit.transform.position, unit.SenseRadius);
            Collider[] inInteractRadius = Physics.OverlapSphere(unit.transform.position, unit.InteractionRadius);
            
            foreach (var hitCollider in inInteractRadius)
            {
                if (hitCollider.GetComponent<Food>() != null)
                {
                    EatFood(hitCollider.GetComponent<Food>());
                    return;
                }
            }
            foreach (var hitCollider in inSenseRadius)
            {
                if (hitCollider.GetComponent<Food>() != null)
                {
                    SearchingForFood(hitCollider.ClosestPoint(unit.transform.position));
                    return;
                }
            }

            Wandering();
        }
        else if (unit.Thirst > unit.ThirstTreshold)
        {
            Collider[] inSenseRadius = Physics.OverlapSphere(unit.transform.position, unit.SenseRadius);
            Collider[] inInteractRadius = Physics.OverlapSphere(unit.transform.position, unit.InteractionRadius);

            foreach (var hitCollider in inInteractRadius)
            {
                if (hitCollider.GetComponent<Drink>() != null)
                {
                    Drink(hitCollider.GetComponent<Drink>());
                    return;
                }
            }
            foreach (var hitCollider in inSenseRadius)
            {
                if (hitCollider.GetComponent<Drink>() != null)
                {
                    SearchingForDrink(hitCollider.ClosestPoint(unit.transform.position));
                    return;
                }
            }

            Wandering();
        }
        else
        {
            if (Random.Range(0,2) == 0)
            {
                IdleBehaviour(idleTime);
            }
            else
            {
                Wandering();
            }
        }
    }
    private void IdleBehaviour(float time)
    {

        CurrentBehaviourState = BehaviourState.Idle;
        behaviurCounter = time;
    }

    private void SearchingForFood( Vector3 foodPosition)
    {
        navMeshAgent.SetDestination(foodPosition);
        CurrentBehaviourState = BehaviourState.SearchingForFood;
    }

    private void SearchingForDrink(Vector3 drinkPosition)
    {
        navMeshAgent.SetDestination(drinkPosition);
        CurrentBehaviourState = BehaviourState.SearchingForDrink;
    }

    private void EatFood(Food food)
    {
        targetedTransform = food.transform;
        currentBehaviourState = BehaviourState.Eating;
    }

    private void Drink(Drink drink)
    {
        targetedTransform = drink.transform;
        currentBehaviourState = BehaviourState.Drinking;
    }

    private void Wandering()
    {
        CurrentBehaviourState = BehaviourState.Wandering;
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        navMeshAgent.SetDestination(finalPosition);
    }
}
