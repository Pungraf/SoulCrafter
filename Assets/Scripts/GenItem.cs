using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Item/GenShard")]
public class GenItem : Item, IUsable
{
    [SerializeField] private GameObject genShardPrefab;

    public string GenName;
    public float GenValue;

    private SingleGen gen;

    public SingleGen Gen
    {
        get { return gen; }
        set { gen = value; }
    }

    public void Use()
    {
        GenShard genShard = Instantiate(genShardPrefab, WorldManager.Instance.Player.transform.position, Quaternion.identity).GetComponent<GenShard>();
        genShard.Initialize(Gen, InitializeInstance(gen));
    }

    public GenItem InitializeInstance(SingleGen singleGen)
    {
        GenItem instace = ScriptableObject.CreateInstance<GenItem>();
        instace.IsStackable = IsStackable;
        instace.MaxStack = MaxStack;
        instace.SpriteName = SpriteName;

        instace.genShardPrefab = genShardPrefab;
        instace.gen = singleGen;

        instace.GenName = singleGen.Type.ToString();
        instace.GenValue = singleGen.Value;

        return instace;
    }
}
