using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitEgg : MonoBehaviour, IInteractable
{
    private static float sekPerDay = 1440f;
    private static float counterUpdateSampling = 1f;

    [SerializeField] private float _hatchingTime;
    [SerializeField] private float _durability;
    [SerializeField] private EggItem eggItem;

    private GameObject evolvedUnitPrefab;

    public GameObject corpseUnitPrefab;

    private GenSample _gens;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateParameters", 0f, counterUpdateSampling);
    }

    public GenSample Gens
    {
        get { return _gens; }
        set { _gens = value; }
    }

    public GameObject EvolvedUnitPrefab
    {
        get { return evolvedUnitPrefab;}
        set { evolvedUnitPrefab = value;}
    }

    // TODO: create IInitializableUnit interface
    public virtual void Initialize(GenSample gen, GameObject evolveUnit, float durability)
    {
        if (gen != null)
        {
            _gens = gen;
        }
        _durability = durability;
        _hatchingTime = gen.Incubation.Value;
        evolvedUnitPrefab = evolveUnit;

    }

    private void Mature()
    {
        if (evolvedUnitPrefab == null)
        {
            Death();
        }
        else
        {
            Unit evolvedUnit = Instantiate(evolvedUnitPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
            UnitController evolvedUnitController = evolvedUnit.controller;
            evolvedUnit.Initialize(Gens, Gens.Vitality.Value, evolvedUnitController.seeker.traversableTags);
            Destroy(gameObject);
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void Interact(PlayerController player)
    {
        eggItem.GenSample = _gens;
        eggItem.EvolvedUnit = evolvedUnitPrefab;

        InventoryManager.Instance.AddItem(eggItem.InitializeInstance(Gens, EvolvedUnitPrefab));
        Destroy(gameObject);
    }

    private void UpdateParameters()
    {
        _hatchingTime -= counterUpdateSampling / sekPerDay;

        if (_hatchingTime <= 0)
        {
            Mature();
        }
    }
}
