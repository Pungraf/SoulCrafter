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
    private List<PackManager> pack = new List<PackManager>();
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

    public List<PackManager> Pack
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
    public delegate void OnVariableChangeDelegate(List<PackManager> newVal);
    public event OnVariableChangeDelegate OnPackChange;

    public PackManager PackLeader
    {
        get { return packLeader; }
        set { packLeader = value; }
    }


    private void PackChangeHandler(List<PackManager> newPack)
    {
        Debug.Log(this.name + " pack leader: " + packLeader + "changed pack members");
    }

    public void DisbandPack()
    {
        foreach (PackManager packMember in Pack)
        {
            packMember.UnsubscribePackChnageHandler();
            packMember.PackLeader = null;
            packMember.HasPack = false;
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
