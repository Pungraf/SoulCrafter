using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera CMCamera;
    [SerializeField]
    private float baseYOffset = 20f;
    [SerializeField]
    private float baseZOffset = -5f;
    [SerializeField]
    private float zoomOutMultplier = 2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
