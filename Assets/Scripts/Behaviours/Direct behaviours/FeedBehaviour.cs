using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(DisposeBehaviour))]
public class FeedBehaviour : BaseBehaviour
{
    private bool isConsuming = false;
    public override void Behave(Action onBehaviourComplete)
    {
        BehaviourStart(onBehaviourComplete);
        if (_unit.Hunger <= _unit.Thirst)
        {
            if(!LookForFood() && _unit.Thirst < 0.1f)
            {
                if(!LookForDrink())
                {
                    if(_unit.IsAgressive)
                    {
                        BehaviourComplete(Behaviour.Hunting);
                    }
                    else
                    {
                        BehaviourComplete(Behaviour.Idle);
                    }
                }
            }
            else
            {
                if(!isAwatingPathCallback && !isConsuming)
                {
                    if (_unit.IsAgressive)
                    {
                        BehaviourComplete(Behaviour.Hunting);
                    }
                    else
                    {
                        BehaviourComplete(Behaviour.Idle);
                    }
                }
            }
        }
        else
        {
            if (!LookForDrink() && _unit.Hunger < 0.1f)
            {
                if(!LookForFood())
                {
                    if (_unit.IsAgressive)
                    {
                        BehaviourComplete(Behaviour.Hunting);
                    }
                    else
                    {
                        BehaviourComplete(Behaviour.Idle);
                    }
                }
            }
            else
            {
                if (!isAwatingPathCallback && !isConsuming)
                {
                    if (_unit.IsAgressive)
                    {
                        BehaviourComplete(Behaviour.Hunting);
                    }
                    else
                    {
                        BehaviourComplete(Behaviour.Idle);
                    }
                }
            }

        }
    }

    protected override float CalculateBehaviourScore()
    {
        float finalScore = 1f - _unit.Hunger;
        if(_unit.Thirst < _unit.Hunger)
        {
            finalScore = 1f - _unit.Thirst;
        }

        if(finalScore > 0.9)
        {
            finalScore += criticalScoreValue;
        }
        return finalScore * behaviourPriority;
    }

    protected bool LookForFood()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Perception.Value);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Reach.Value);

        foreach (var hitCollider in inInteractRadius)
        {
            Food food = hitCollider.GetComponent<Food>();
            if (food != null)
            {
                if (_unit.edibleFood.Contains(food.foodType))
                {
                    isAwatingPathCallback = false;
                    StartCoroutine(Eat(food));
                    return true;
                }
            }

        }

        Transform potentialFoodTarget = null;
        List<Transform> foodTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build lists with proper types
            Transform sensedTransform = hitCollider.transform;
            Food foodTransform = sensedTransform.GetComponent<Food>();

            if (foodTransform != null && _unit.edibleFood.Contains(foodTransform.foodType))
            {
                foodTargets.Add(sensedTransform);
            }
        }

        //Search for closest food
        potentialFoodTarget = _unitController.FindClosestTransformPath(foodTargets);
        if (potentialFoodTarget != null)
        {
            SearchingForFood(potentialFoodTarget.GetComponent<Collider>().ClosestPoint(transform.position));
            return true;
        }

        return false;
    }

    protected void SearchingForFood(Vector3 foodPosition)
    {
        isAwatingPathCallback = true;
        _unitController.MoveUnit(foodPosition);
    }

    IEnumerator Eat(Food food)
    {
        isConsuming = true;
        while(food != null && _unit.Hunger < 0.95f)
        {
            food.Eat(_unit);
            yield return new WaitForSeconds(1f);
        }
        isConsuming = false;

        if (_unit.IsAgressive)
        {
            _unit.Anger -= 1f;
        }

        BehaviourComplete();
    }

    protected bool LookForDrink()
    {
        Collider[] inSenseRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Perception.Value);
        Collider[] inInteractRadius = Physics.OverlapSphere(transform.position, _unit.Gens.Reach.Value);

        foreach (var hitCollider in inInteractRadius)
        {
            Drink drink = hitCollider.GetComponent<Drink>();
            if (drink != null)
            {
                isAwatingPathCallback = false;
                StartCoroutine(Drink(drink));
                return true;
            }
        }

        Transform potentialDrinkTarget = null;
        List<Transform> drinkTargets = new List<Transform>();

        foreach (var hitCollider in inSenseRadius)
        {
            //Build list with proper type
            Drink sensedDrink = hitCollider.GetComponent<Drink>();
            if (sensedDrink != null)
            {
                drinkTargets.Add(sensedDrink.transform);
            }
        }

        //Search for closest Drink
        potentialDrinkTarget = _unitController.FindClosestTransformPath(drinkTargets);

        if (potentialDrinkTarget != null)
        {
            SearchingForDrink(potentialDrinkTarget.GetComponent<Collider>().ClosestPoint(transform.position));
            return true;
        }

        return false;
    }

    protected void SearchingForDrink(Vector3 drinkPosition)
    {
        isAwatingPathCallback = true;
        _unitController.MoveUnit(drinkPosition);
    }

    IEnumerator Drink(Drink drink)
    {
        isConsuming = true;
        while (drink != null && _unit.Thirst < 0.95f)
        {
            drink.Drinking(_unit);
            yield return new WaitForSeconds(1f);
        }
        isConsuming = false;
        BehaviourComplete();
    }
}
