using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionSensor : MonoBehaviour
{
    [SerializeField] private SphereCollider interactionCollider;
    [SerializeField] private Camera UICamera;

    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction Interact;

    private bool actionPanelIsActive = false;
    private Transform closestTransform;
    private Transform newClosestTransform;

    private void Awake()
    {
        PlayerActionMap = InputActions.FindActionMap("Player");
        Interact = PlayerActionMap.FindAction("Interract");
        Interact.started += HandleInterractAction;
        Interact.Enable();
        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.GetComponent<IInteractable>() != null)
        {
            actionPanelIsActive = true;
        }
    }

    private void HandleInterractAction(InputAction.CallbackContext Context)
    {
        if(actionPanelIsActive && closestTransform != null)
        {
            closestTransform.GetComponent<IInteractable>().Interact();
        }
    }

    private Transform ClosestObjectInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionCollider.radius);
        float closestDistance = float.MaxValue;
        Transform closestTransform = null;

        foreach(Collider collider in hitColliders)
        {
            if (collider.transform.GetComponent<IInteractable>() == null) continue;
            if(collider.transform != transform && Vector3.Distance(transform.position, collider.transform.position) < closestDistance)
            {
                closestTransform = collider.transform;
            }
        }

        return closestTransform;
    }

    private void Update()
    {
        if(actionPanelIsActive)
        {
            newClosestTransform = ClosestObjectInRange();
            if(newClosestTransform != null)
            {
                if (newClosestTransform == closestTransform)
                {
                    UIManager.Instance.SetActionButtonPanelPosition(UICamera.WorldToScreenPoint(closestTransform.position));
                }
                else
                {
                    closestTransform = newClosestTransform;
                    UIManager.Instance.EnableActionButtonPanel(UICamera.WorldToScreenPoint(closestTransform.position), "E", "Pick Up");
                }
            }
            else
            {
                actionPanelIsActive = false;
                closestTransform = null;
                UIManager.Instance.DisableActionButtonPanel();
            }
        }
    }
}
