using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Item/Egg")]
public class EggItem : Item, IUsable
{
    [SerializeField] private GameObject eggPrefab;
    private GenSample _genSample;
    private GameObject _evolvedUnit;

    public GenSample GenSample
    {
        get { return _genSample; }
        set { _genSample = value; }
    }

    public GameObject EvolvedUnit
    {
        get { return _evolvedUnit; }
        set { _evolvedUnit = value; }
    }

    public void Use()
    {
        UnitEgg egg = Instantiate(eggPrefab, WorldManager.Instance.Player.transform.position, Quaternion.identity).GetComponent<UnitEgg>();
        egg.Initialize(_genSample, _evolvedUnit, _genSample.MaxHealth, _genSample.HungerTreshold, _genSample.ThirstTreshold);
    }
}
