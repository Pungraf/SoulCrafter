using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager Instance { get; private set; }

    [SerializeField] private GameObject window_Graph;
    [SerializeField] private float populationSamplingTime;
    [SerializeField] private Window_Graph window_graph;

    [SerializeField] private Toggle togglePopulation;
    [SerializeField] private Toggle toggleLifeTime;
    [SerializeField] private Toggle toggleMaxHealth;
    [SerializeField] private Toggle toggleHungerResistance;
    [SerializeField] private Toggle toggleThirstResistance;
    [SerializeField] private Toggle toggleWalkSpeed;
    [SerializeField] private Toggle toggleSenseRadius;
    [SerializeField] private Toggle toggleConsumingSpeed;
    [SerializeField] private Toggle togglePregnancyTime;
    [SerializeField] private Toggle toggleOffspringTime;
    [SerializeField] private Toggle toggleOffspringPopulation;
    [SerializeField] private Toggle toggleAttractivness;

    public Transform WispsHolder;

    private float samplingCounter = 0;
    private List<int> populationTimeLine = new List<int>();

    private List<GenMerger> genTimeLine = new List<GenMerger>();



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
            GenMerger newSample = GenManager.Instance.GetAvarageGen();
            if(newSample.SamplesCount != 0)
            {
                genTimeLine.Add(GenManager.Instance.GetAvarageGen());
            }
            else
            {
                //TODO: Fill with empty sample without /0
            }
            samplingCounter = populationSamplingTime;
            if (window_Graph.activeSelf == true)
            {
                UpdateProperGraph();
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            window_Graph.SetActive(!window_Graph.activeSelf);
        }

    }

    //TODO: Ugly, make it shorter.
    public void UpdateProperGraph()
    {
        if(window_Graph.activeSelf == false)
        {
            return;
        }

        if(togglePopulation.isOn)
        {
            window_graph.ShowGraph(populationTimeLine, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " Qt");
        }
        else if(toggleLifeTime.isOn)
        {
            List<int> genList = new List<int>();
            foreach(GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.LifeTime));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " S");
        }
        else if (toggleMaxHealth.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.MaxHealth));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " Hp");
        }
        else if (toggleHungerResistance.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.HungerResistance * 100));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " %");
        }
        else if (toggleThirstResistance.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.ThirstResistance * 100));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " %");
        }
        else if (toggleWalkSpeed.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.WalkSpeed));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " m/s");
        }
        else if (toggleSenseRadius.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.SenseRadius));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " m");
        }
        else if (toggleConsumingSpeed.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.ConsumingSpeed));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " f/s");
        }
        else if (togglePregnancyTime.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.PregnancyTime));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " s");
        }
        else if (toggleOffspringTime.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.OffspringTime));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " s");
        }
        else if (toggleOffspringPopulation.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.OffspringMaxPopulation));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " Qt");
        }
        else if (toggleAttractivness.isOn)
        {
            List<int> genList = new List<int>();
            foreach (GenMerger gen in genTimeLine)
            {
                genList.Add(Convert.ToInt32(gen.Attractivness * 100));
            }
            window_graph.ShowGraph(genList, 60, (int _i) => (_i + 1) + " '", (float _f) => _f + " %");
        }
        else
        {
            //Debug.LogError("Missing tab requested");
        }
    }

    private void Unit_OnAnyUnitSpawn(object sender, EventArgs e)
    {
        
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        
    }
}
