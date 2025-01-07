using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarPanel : MonoBehaviour, IInteractable
{
    private bool altarPanelIsActive;
    private Transform playerTransform;
    private float disablePanelDistance = 3f;

    public void Interact(PlayerController player)
    {
        altarPanelIsActive = true;
        UIManager.Instance.ToggleAltarPanelUI(true);
    }

    private void Start()
    {
        playerTransform = WorldManager.Instance.Player.transform;
    }

    private void Update()
    {
        if(altarPanelIsActive)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if(distance > disablePanelDistance)
            {
                altarPanelIsActive = false;
                UIManager.Instance.ToggleAltarPanelUI(false);
            }
        }
    }
}
