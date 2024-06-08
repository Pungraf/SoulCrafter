using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitEgg : MonoBehaviour, IInteractable
{
    [SerializeField] private float _hatchingTime;
    [SerializeField] private float _durability;
    [SerializeField] private EggItem eggItem;

    private GameObject evolvedUnitPrefab;

    public GameObject corpseUnitPrefab;

    private GenSample _gens;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _hatchingTime -= Time.deltaTime;

        if(_hatchingTime <= 0 )
        {
            Mature();
        }
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
    public virtual void Initialize(GenSample gen, GameObject evolveUnit, float health = 100, float hunger = 0, float thirst = 0)
    {
        if (gen != null)
        {
            _gens = gen;
        }
        _durability = health;
        _hatchingTime = gen.IncubationTime;
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
            evolvedUnit.Initialize(Gens, Gens.MaxHealth, Gens.HungerTreshold, Gens.ThirstTreshold);
            Destroy(gameObject);
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public void Interact()
    {
        eggItem.GenSample = _gens;
        eggItem.EvolvedUnit = evolvedUnitPrefab;

        InventoryManager.Instance.AddItem(eggItem);
        Destroy(gameObject);
    }
}
