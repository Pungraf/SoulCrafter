using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager Instance { get; private set; }

    [SerializeField] private GameObject window_Graph;

    public Transform WispsHolder;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one SoulsManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            window_Graph.SetActive(!window_Graph.activeSelf);
        }
    }
}
