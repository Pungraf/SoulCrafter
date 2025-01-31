using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public AbilitySet abilitySet;
    public Animator playerAC;

    private Ability primaryAbility;

    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction ActionOne;
    private InputAction ActionTwo;
    private InputAction ActionThree;
    private InputAction ActionFour;
    private InputAction PrimaryAbilityInput;


    private PlayerPackManager packManager;
    public PlayerPackManager PackManager { get { return packManager; } }

    private void Awake()
    {
        PlayerActionMap = InputActions.FindActionMap("Player");

        ActionOne = PlayerActionMap.FindAction("ActionOne");
        ActionTwo = PlayerActionMap.FindAction("ActionTwo");
        ActionThree = PlayerActionMap.FindAction("ActionThree");
        ActionFour = PlayerActionMap.FindAction("ActionFour");
        PrimaryAbilityInput = PlayerActionMap.FindAction("PrimaryAbility");

        ActionOne.started += HandleActinOneAction;
        ActionTwo.started += HandleActinTwoAction;
        ActionThree.started += HandleActinThreeAction;
        ActionFour.started += HandleActinFourAction;
        PrimaryAbilityInput.started += HandleActinPrimaryAbility;

        ActionOne.Enable();
        ActionTwo.Enable();
        ActionThree.Enable();
        ActionFour.Enable();
        PrimaryAbilityInput.Enable();

        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    

    // Start is called before the first frame update
    void Start()
    {
        WorldManager.Instance.Player = this.gameObject;
        packManager = GetComponent<PlayerPackManager>();

        primaryAbility = abilitySet.GetAbilityByName("Swing");
        playerAC = GetComponent<Animator>();
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

    private void HandleActinPrimaryAbility(InputAction.CallbackContext obj)
    {
        primaryAbility.Activate(this);
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
                packMember.UnitController.Brain.ClearAllPeristentBehaviours();
                Vector3 destination = MouseWorld.GetPosition();
                packMember.UnitController.ForceBehaviour(BaseBehaviour.Behaviour.Move, destination);
            }
        }
    }

    private void CommandeGather()
    {
        if (PackManager.Pack.Count == 0)
        {
            Debug.Log("No members to Gather");
            return;
        }
        foreach (UnitPackManager packMember in PackManager.Pack)
        {
            if (packManager != null)
            {
                packMember.UnitController.Brain.ClearAllPeristentBehaviours();
                Vector3 destination = transform.position;
                packMember.UnitController.ForceBehaviour(BaseBehaviour.Behaviour.Move, destination);
            }
        }
    }

    private void CommandeStay()
    {
        if (PackManager.Pack.Count == 0)
        {
            Debug.Log("No members to Guard");
            return;
        }
        foreach (UnitPackManager packMember in PackManager.Pack)
        {
            if (packManager != null)
            {
                packMember.UnitController.Brain.ClearAllPeristentBehaviours();
                Vector3 destination = MouseWorld.GetPosition();
                packMember.UnitController.ForceBehaviour(BaseBehaviour.Behaviour.Guarding, destination);
            }
        }
    }

    private void CommandeDisband()
    {
        PackManager.DisbandPack();
    }
}
