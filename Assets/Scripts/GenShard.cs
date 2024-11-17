using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenShard : MonoBehaviour, IInteractable
{
    [SerializeField] private GenItem genItem;

    private SingleGen gen;

    public GenItem GenItem
    {
        get { return genItem; }
        set { genItem = value; }
    }

    public SingleGen Gen
    {
        get { return gen; }
        set { gen = value; }
    }

    public virtual void Initialize(SingleGen gen, GenItem genItem)
    {
        Gen = gen;
        this.genItem = genItem;
    }

    public void Interact(PlayerController player)
    {
        genItem.Gen = Gen;

        InventoryManager.Instance.AddItem(genItem.InitializeInstance(Gen));
        Destroy(gameObject);
    }
}
