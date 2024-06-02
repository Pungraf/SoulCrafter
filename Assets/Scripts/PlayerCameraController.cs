using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset InputActions;
    private InputActionMap PlayerActionMap;
    private InputAction Zoom;

    [SerializeField]
    private CinemachineVirtualCamera standardCMCamera;
    private CinemachineTransposer standardCMCameraCMTransposer;
    [SerializeField]
    private CinemachineVirtualCamera zoomCMCamera;
    private CinemachineTransposer zoomCMCameraCMTransposer;
    [SerializeField]
    private Vector3 baseOffset;
    [SerializeField]
    private float zoomOutMultplier = 2f;

    private void Awake()
    {
        PlayerActionMap = InputActions.FindActionMap("Player");
        Zoom = PlayerActionMap.FindAction("Zoom");
        Zoom.started += HandleZoomActionStarted;
        Zoom.canceled += HandleZoomActionCanceled;
        Zoom.Enable();
        PlayerActionMap.Enable();
        InputActions.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        standardCMCameraCMTransposer = standardCMCamera.GetCinemachineComponent<CinemachineTransposer>();
        standardCMCameraCMTransposer.m_FollowOffset = baseOffset;
        standardCMCameraCMTransposer = zoomCMCamera.GetCinemachineComponent<CinemachineTransposer>();
        standardCMCameraCMTransposer.m_FollowOffset = baseOffset * zoomOutMultplier;
    }

    private void HandleZoomActionStarted(InputAction.CallbackContext Context)
    {
        standardCMCamera.gameObject.SetActive(false);
        zoomCMCamera.gameObject.SetActive(true);
    }
    private void HandleZoomActionCanceled(InputAction.CallbackContext Context)
    {
        standardCMCamera.gameObject.SetActive(true);
        zoomCMCamera.gameObject.SetActive(false);
    }

}
