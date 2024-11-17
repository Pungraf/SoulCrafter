using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Item/Egg")]
public class EggItem : Item, IUsable
{
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private GameObject eggVisualsPrefab;

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

    public GameObject EggPrefab
    {
        get { return eggPrefab; }
        set { eggPrefab = value; }
    }

    public GameObject EggVisualsPrefab
    {
        get { return eggVisualsPrefab; }
        set { eggVisualsPrefab = value; }
    }

    public void Use()
    {
        UnitEgg egg = Instantiate(eggPrefab, WorldManager.Instance.Player.transform.position, Quaternion.identity).GetComponent<UnitEgg>();
        egg.Initialize(_genSample, _evolvedUnit, _genSample.Vitality.Value);
    }
    public EggItem InitializeInstance(GenSample genSample, GameObject evolvedUnit)
    {
        EggItem instace = ScriptableObject.CreateInstance<EggItem>();
        instace.IsStackable = IsStackable;
        instace.MaxStack = MaxStack;
        instace.SpriteName = SpriteName;

        instace.eggPrefab = eggPrefab;
        instace.eggVisualsPrefab = eggVisualsPrefab;
        instace.GenSample = genSample;
        instace.EvolvedUnit = evolvedUnit;

        return instace;
    }
}
