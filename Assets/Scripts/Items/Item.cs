using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStack;
    [SerializeField] private string spriteName;

    public bool IsStackable
    {
        get => isStackable;
        set
        {
            isStackable = value;
        }
    }

    public int MaxStack
    {
        get => maxStack;
        set
        {
            maxStack = value;
        }
    }

    public string SpriteName
    {
        get => spriteName;
        set
        {
            spriteName = value;
        }
    }
    public Sprite GetSprite()
    {
        return UIAssets.Instance.GetSprite(spriteName);
    }

    public Item InitializeInstance()
    {
        return CreateInstance<Item>();
    }
}
