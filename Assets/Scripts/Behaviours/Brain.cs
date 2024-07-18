using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private BaseBehaviour currentBehaviour;
    public BaseBehaviour CurrentBehaviour
    {
        get { return currentBehaviour; }
        set { currentBehaviour = value; }
    }
    [SerializeField] protected List<BaseBehaviour> behavioursList;
    private UnitController _unitController;
    // Start is called before the first frame update
    void Awake()
    {
        var behaviours = GetComponents<BaseBehaviour>();
        behavioursList.Clear();
        foreach (BaseBehaviour behaviour in behaviours)
        {
            behavioursList.Add(behaviour);
        }
    }

    private void Start()
    {
        _unitController = GetComponentInParent<UnitController>();
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

    public void ForceNextBehaviour(BaseBehaviour.Behaviour behaviourType, bool keepTarget = false)
    {
        foreach (BaseBehaviour behaviour in behavioursList)
        {
            if (behaviour.IsActive)
            {
                behaviour.BehaviourComplete(behaviourType, keepTarget);
                return;
            }
        }
        _unitController.ChooseBehaviour();
    }
}
