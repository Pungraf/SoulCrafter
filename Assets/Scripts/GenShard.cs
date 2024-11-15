using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenShard : MonoBehaviour, IInteractable
{
    [SerializeField] private GenItem genItem;

    private SingleGen gen;

    public SingleGen Gen
    {
        get { return gen; }
        set { gen = value; }
    }

    public virtual void Initialize(SingleGen gen)
    {
        Gen = gen;
    }

    public void Interact(PlayerController player)
    {
        genItem.Gen = this.Gen;

        InventoryManager.Instance.AddItem(genItem.InitializeInstance(Gen));
        Destroy(gameObject);
    }
}
