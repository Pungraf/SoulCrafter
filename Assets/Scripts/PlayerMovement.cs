using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction Movement;

    private NavMeshAgent Agent;
    [SerializeField]
    [Range(0f, 0.99f)]
    private float Smoothing = 0.25f;
    [SerializeField]
    private float TargetLerpSpeed = 1;

    private Vector3 TargetDirection;
    private float LerpTime = 0;
    private Vector3 LastDirection;
    private Vector3 MovementVector;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        PlayerActionMap = InputActions.FindActionMap("Player");
        Movement = PlayerActionMap.FindAction("Move");
        Movement.started += HandleMovementAction;
        Movement.canceled += HandleMovementAction;
        Movement.performed += HandleMovementAction;
        Movement.Enable();
        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    private void HandleMovementAction(InputAction.CallbackContext Context)
    {
        Vector2 input = Context.ReadValue<Vector2>();
        MovementVector = new Vector3(input.x, 0, input.y);
    }

    private void Update()
    {
        MovementVector.Normalize();
        if(MovementVector != LastDirection)
        {
            LerpTime = 0;
        }
        LastDirection = MovementVector;
        TargetDirection = Vector3.Lerp(TargetDirection, MovementVector, Mathf.Clamp01(LerpTime * TargetLerpSpeed * (1 - Smoothing)));

        Agent.Move(TargetDirection * Agent.speed * Time.deltaTime);

        Vector3 lookDirection = MovementVector;
        if(lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection), Mathf.Clamp01(LerpTime * TargetLerpSpeed * (1 - Smoothing)));
        }

        LerpTime += Time.deltaTime;
    }

}
