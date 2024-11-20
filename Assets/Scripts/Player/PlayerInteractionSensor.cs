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
    private PlayerController playerController;

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

        playerController = GetComponentInParent<PlayerController>();
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
            closestTransform.GetComponent<IInteractable>().Interact(playerController);
        }
    }

    private Transform ClosestObjectInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionCollider.radius);
        float closestDistance = float.MaxValue;
        Transform closestTransform = null;

        foreach(Collider collider in hitColliders)
        {
            Transform colTransform = collider.transform;
            if (colTransform.GetComponent<IInteractable>() == null) continue;
            if (colTransform.CompareTag("Unit") && playerController.PackManager.Pack.Contains(colTransform.GetComponent<UnitPackManager>())) continue;
            if (colTransform != transform && Vector3.Distance(transform.position, colTransform.position) < closestDistance)
            {
                closestTransform = colTransform;
            }
        }

        return closestTransform;
    }

    //TODO: changer for Ticker Update ( Performance )
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
                    if (closestTransform.CompareTag("Unit") && !playerController.PackManager.Pack.Contains(closestTransform.GetComponent<UnitPackManager>()))
                    {
                        UIManager.Instance.EnableActionButtonPanel(UICamera.WorldToScreenPoint(closestTransform.position), "E", "Take over");
                    }
                    else if(closestTransform.CompareTag("Pickable"))
                    {
                        UIManager.Instance.EnableActionButtonPanel(UICamera.WorldToScreenPoint(closestTransform.position), "E", "Pick Up");
                    }
                    else if (closestTransform.CompareTag("Interractable"))
                    {
                        UIManager.Instance.EnableActionButtonPanel(UICamera.WorldToScreenPoint(closestTransform.position), "E", "Interract");
                    }
                }
            }
            else
            {
                actionPanelIsActive = false;
                closestTransform = null;
                UIManager.Instance.DisableActionButtonPanel();
                UIManager.Instance.DisableAllWindows();
            }
        }
    }
}
