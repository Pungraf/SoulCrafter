using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpliceCore : MonoBehaviour, IInteractable
{
    [SerializeField] private EggSlot eggSlot;
    [SerializeField] private SingleGenSlot singleGenSlot;

    public void Interact(PlayerController player)
    {
        UIManager.Instance.SpliceCorePanelChangeState();
    }

    public void SpliceGenIntoEgg()
    {
        if(eggSlot.ItemInSlot == null || singleGenSlot.ItemInSlot == null)
        {
            Debug.Log("Splice component missing.");
        }
        else
        {
            Debug.Log("Splicing gens");
        }
    }
}
