using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarPanel : MonoBehaviour, IInteractable
{
    [SerializeField] Altar altar;

    public void Interact(PlayerController player)
    {
        UIManager.Instance.AltarPanelChangeState();
    }
}
