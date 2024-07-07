using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brain : MonoBehaviour
{
    [SerializeField] protected List<BaseBehaviour> behavioursList;
    // Start is called before the first frame update
    void Start()
    {
        var behaviours = GetComponents<BaseBehaviour>();
        behavioursList.Clear();
        foreach (BaseBehaviour behaviour in behaviours)
        {
            behavioursList.Add(behaviour);
        }
    }

    public BaseBehaviour GetFirstBehaviour()
    {
        foreach (BaseBehaviour behaviour in behavioursList)
        {
            behaviour.CalculateCurrentBehaviourScore();
        }

        behavioursList = behavioursList.OrderByDescending(x => x.currnetBehaviourScore).ToList();
        return behavioursList[0];
    }

    public BaseBehaviour GetBehaviourByType(BaseBehaviour.Behaviour type)
    {
        foreach (BaseBehaviour behaviour in behavioursList)
        {
            if(behaviour.behaviourType == type)
            {
                return behaviour;
            }
        }
        return null;
    }
}
