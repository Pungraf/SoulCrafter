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
    public Transform WolfsHolder;

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


    private void Unit_OnAnyUnitSpawn(object sender, EventArgs e)
    {
        
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        
    }
}
