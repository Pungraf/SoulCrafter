using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpliceCore : MonoBehaviour, IInteractable
{
    private bool spliceCorePanelIsActive;
    private Transform playerTransform;
    private float disablePanelDistance = 3f;

    private EggItem eggInCore;
    private GameObject eggVisual;

    private GenItem genInCore;

    private void Start()
    {
        playerTransform = WorldManager.Instance.Player.transform;

        if (UIManager.Instance.M_SpliceButton != null)
        {
            UIManager.Instance.M_SpliceButton.clicked += SpliceGenIntoEgg;
        }

        UIManager.Instance.M_EggSlot.SetSlotRestriction(new List<Item.ItemType> { Item.ItemType.Egg });
        UIManager.Instance.M_GenSlot.SetSlotRestriction(new List<Item.ItemType> { Item.ItemType.SoulShard });

        UIManager.Instance.M_EggSlot.OnItemChanged += EggSlot_OnSlotItemChanged;
        UIManager.Instance.M_GenSlot.OnItemChanged += SingleGenSlot_OnSlotItemChanged;

    }

    private void Update()
    {
        if (spliceCorePanelIsActive)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance > disablePanelDistance)
            {
                spliceCorePanelIsActive = false;
                UIManager.Instance.ToggleAltarPanelUI(false);
            }
        }
    }

    private void EggSlot_OnSlotItemChanged(UI_InventorySlot slot, Item newItem)
    {
        if(newItem != null)
        {
            eggInCore = (EggItem)newItem;
            eggVisual = Instantiate(eggInCore.EggVisualsPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Destroy(eggVisual);
            eggInCore = null;
        }
    }

    private void SingleGenSlot_OnSlotItemChanged(UI_InventorySlot slot, Item newItem)
    {
        if (newItem != null)
        {

            genInCore = (GenItem)newItem;
        }
        else
        {
            genInCore = null;
        }
    }

    public void Interact(PlayerController player)
    {
        spliceCorePanelIsActive = true;
        UIManager.Instance.ToggleSpliceCorePanelUI(true);
    }

    public void SpliceGenIntoEgg()
    {
        if (eggInCore == null || genInCore == null)
        {
            Debug.Log("Splice component missing.");
        }
        else
        {
            ModifieGenValue(eggInCore.GenSample, genInCore.Gen);

            UIManager.Instance.M_GenSlot.RemoveItemFromSlot();
        }
    }

    public void ModifieGenValue(GenSample modifiedGen, SingleGen newSingleGen)
    {
        switch (newSingleGen.Type)
        {
            case SingleGen.GenType.LifeSpan:
                modifiedGen.LifeSpan = newSingleGen;
                break;
            case SingleGen.GenType.Incubation:
                modifiedGen.Incubation = newSingleGen;
                break;
            case SingleGen.GenType.Vitality:
                modifiedGen.Vitality = newSingleGen;
                break;
            case SingleGen.GenType.Speed:
                modifiedGen.Speed = newSingleGen;
                break;
            case SingleGen.GenType.Strength:
                modifiedGen.Strength = newSingleGen;
                break;
            case SingleGen.GenType.Satiety:
                modifiedGen.Satiety = newSingleGen;
                break;
            case SingleGen.GenType.Hydration:
                modifiedGen.Hydration = newSingleGen;
                break;
            case SingleGen.GenType.Ingestion:
                modifiedGen.Ingestion = newSingleGen;
                break;
            case SingleGen.GenType.Urge:
                modifiedGen.Urge = newSingleGen;
                break;
            case SingleGen.GenType.Reach:
                modifiedGen.Reach = newSingleGen;
                break;
            case SingleGen.GenType.Perception:
                modifiedGen.Perception = newSingleGen;
                break;
            case SingleGen.GenType.Fecundity:
                modifiedGen.Fecundity = newSingleGen;
                break;
            case SingleGen.GenType.Attractiveness:
                modifiedGen.Attractiveness = newSingleGen;
                break;
            case SingleGen.GenType.Gestation:
                modifiedGen.Gestation = newSingleGen;
                break;
            case SingleGen.GenType.Fertility:
                modifiedGen.Fertility = newSingleGen;
                break;
            default:
                Debug.LogError($"Unknown GenType: {newSingleGen.Type}");
                break;
        }
    }

}
