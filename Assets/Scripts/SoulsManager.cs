using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager Instance { get; private set; }

    [SerializeField] private GameObject window_Graph;
    [SerializeField] private float populationSamplingTime;
    [SerializeField] private Window_Graph window_graph;

    public Transform WispsHolder;

    private float samplingCounter = 0;
    private List<int> populationTimeLine = new List<int>();

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
        Unit.OnAnyUnitSpawn += Unit_OnAnyUnitSpawn;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    // Update is called once per frame
    void Update()
    {
        samplingCounter -= Time.deltaTime;
        if(samplingCounter <= 0)
        {
            populationTimeLine.Add(WispsHolder.childCount);
            samplingCounter = populationSamplingTime;
            if (window_Graph.activeSelf == true)
            {
                window_graph.ShowGraph(populationTimeLine, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " Qt");
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            window_Graph.SetActive(!window_Graph.activeSelf);
            if(window_Graph.activeSelf == true)
            {
                window_graph.ShowGraph(populationTimeLine, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " Qt");
            }
        }
    }

    private void Unit_OnAnyUnitSpawn(object sender, EventArgs e)
    {
        
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        
    }
}
