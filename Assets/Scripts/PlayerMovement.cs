using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(AIPath))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction Movement;
    private InputAction Sprint;

    public Seeker seeker;
    public AIPath aIPath;

    [SerializeField]
    [Range(0f, 0.99f)]
    private float Smoothing = 0.25f;
    [SerializeField]
    private float TargetLerpSpeed = 1;

    private Vector3 TargetDirection;
    private float LerpTime = 0;
    private Vector3 LastDirection;
    private Vector3 MovementVector;

    [SerializeField]
    private float Speed = 3f;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();
        PlayerActionMap = InputActions.FindActionMap("Player");
        Movement = PlayerActionMap.FindAction("Move");
        Movement.started += HandleMovementAction;
        Movement.canceled += HandleMovementAction;
        Movement.performed += HandleMovementAction;
        Movement.Enable();
        Sprint = PlayerActionMap.FindAction("Sprint");
        Sprint.started += HandleSprintActionStarted;
        Sprint.canceled += HandleSprintActionCanceld;
        Sprint.Enable();
        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    private void Start()
    {
        aIPath.maxSpeed = Speed;
    }

    private void HandleMovementAction(InputAction.CallbackContext Context)
    {
        Vector2 input = Context.ReadValue<Vector2>();
        MovementVector = new Vector3(input.x, 0, input.y);
    }

    private void HandleSprintActionStarted(InputAction.CallbackContext Context)
    {
        aIPath.maxSpeed = Speed * 2;
    }

    private void HandleSprintActionCanceld(InputAction.CallbackContext Context)
    {
        aIPath.maxSpeed = Speed;
    }

    private void Update()
    {
        Move();
        Rotate();

        LerpTime += Time.deltaTime;
    }

    private void Move()
    {
        MovementVector.Normalize();
        aIPath.destination = transform.position + MovementVector;
    }

    private void Rotate()
    {
        Vector3 ScreenPos = MouseWorld.GetPosition();

        Vector3 lookDirection = ScreenPos - transform.position;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection), Mathf.Clamp01(LerpTime * TargetLerpSpeed * (1 - Smoothing)));
        }
    }
}
