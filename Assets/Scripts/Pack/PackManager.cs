using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class PackManager : MonoBehaviour
{
    [SerializeField]
    private bool hasPack;
    [SerializeField]
    private bool isLeader;
    [SerializeField]
    private List<UnitPackManager> pack = new List<UnitPackManager>();
    [SerializeField]
    private PackManager packLeader;

    [SerializeField]
    private int packSize;

    public bool HasPack
    {
        get { return hasPack; }
        set { hasPack = value; }
    }

    public bool IsLeader
    {
        get { return isLeader; }
        set { isLeader = value; }
    }

    public int PackSize
    {
        get { return packSize; }
    }

    public List<UnitPackManager> Pack
    {
        get { return pack; }
        set
        {
            if (pack == value) return;
            pack = value;
            if (OnPackChange != null)
            {
                OnPackChange(pack);
                pack.RemoveAll(item => item == null);
            }
        }
    }
    public delegate void OnVariableChangeDelegate(List<UnitPackManager> newVal);
    public event OnVariableChangeDelegate OnPackChange;

    public PackManager PackLeader
    {
        get { return packLeader; }
        set { packLeader = value; }
    }


    private void PackChangeHandler(List<UnitPackManager> newPack)
    {
        Debug.Log(this.name + " pack leader: " + packLeader + "changed pack members");
    }

    public virtual void DisbandPack()
    {
        foreach (UnitPackManager packMember in Pack)
        {
            packMember.UnsubscribePackChnageHandler();
            packMember.PackLeader = null;
            packMember.HasPack = false;
            packMember.UnitController.Brain.ClearAllPeristentBehaviours();
        }
        Pack.Clear();
    }

    public void UnsubscribePackChnageHandler()
    {
        if(PackLeader != null)
        {
            PackLeader.OnPackChange -= PackChangeHandler;
        }
    }

    public void SubscribePackChnageHandler(PackManager packLeader)
    {
        packLeader.OnPackChange += PackChangeHandler;
    }
}
