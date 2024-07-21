using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction ActionOne;
    private InputAction ActionTwo;
    private InputAction ActionThree;
    private InputAction ActionFour;

    private PlayerPackManager packManager;
    public PlayerPackManager PackManager { get { return packManager; } }

    private void Awake()
    {
        PlayerActionMap = InputActions.FindActionMap("Player");

        ActionOne = PlayerActionMap.FindAction("ActionOne");
        ActionTwo = PlayerActionMap.FindAction("ActionTwo");
        ActionThree = PlayerActionMap.FindAction("ActionThree");
        ActionFour = PlayerActionMap.FindAction("ActionFour");

        ActionOne.started += HandleActinOneAction;
        ActionTwo.started += HandleActinTwoAction;
        ActionThree.started += HandleActinThreeAction;
        ActionFour.started += HandleActinFourAction;

        ActionOne.Enable();
        ActionTwo.Enable();
        ActionThree.Enable();
        ActionFour.Enable();

        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        WorldManager.Instance.Player = this.gameObject;
        packManager = GetComponent<PlayerPackManager>();
    }

    private void HandleActinOneAction(InputAction.CallbackContext obj)
    {
        CommandeMove();
    }

    private void HandleActinTwoAction(InputAction.CallbackContext obj)
    {
        CommandeGather();
    }

    private void HandleActinThreeAction(InputAction.CallbackContext obj)
    {
        CommandeStay();
    }

    private void HandleActinFourAction(InputAction.CallbackContext obj)
    {
        CommandeDisband();
    }

    private void CommandeMove()
    {
        if(PackManager.Pack.Count == 0)
        {
            Debug.Log("No members to move");
            return;
        }
        foreach (UnitPackManager packMember in PackManager.Pack)
        {
            if(packManager != null)
            {
                Vector3 destination = MouseWorld.GetPosition();
                packMember.UnitController.ForceBehaviour(BaseBehaviour.Behaviour.Move, destination);
            }
        }
    }

    private void CommandeGather()
    {
        if (PackManager.Pack.Count == 0)
        {
            Debug.Log("No members to move");
            return;
        }
        foreach (UnitPackManager packMember in PackManager.Pack)
        {
            if (packManager != null)
            {
                Vector3 destination = transform.position;
                packMember.UnitController.ForceBehaviour(BaseBehaviour.Behaviour.Move, destination);
            }
        }
    }

    private void CommandeStay()
    {
        Debug.Log("Stay commanded");
    }

    private void CommandeDisband()
    {
        PackManager.DisbandPack();
    }
}
