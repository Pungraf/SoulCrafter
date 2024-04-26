using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenManager : MonoBehaviour
{
    public static GenManager Instance { get; private set; }

    private GenMerger genMerged;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GenManager! " + transform + " - " + Instance);
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
        
    }

    public GenMerger GetAvarageGen()
    {
        genMerged = new GenMerger();
        foreach (Unit unit in SoulsManager.Instance.WispsHolder.GetComponentsInChildren<Unit>())
        {
            if (unit.IsAdult)
            {
                genMerged.SamplesCount++;
                Reflection.AddPropertiesValues(unit.Gens, genMerged);
            }
        }
        return genMerged;
    }
}
