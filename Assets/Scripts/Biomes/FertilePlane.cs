using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilePlane : MonoBehaviour
{
    private static System.Random random = new System.Random();

    public List<SoulTree> soulTrees = new List<SoulTree> {null, null};

    [SerializeField] private float BaseFertilization = 0.4f;
    [SerializeField] GameObject plantPrefab;

    [SerializeField] private float fertilization;

    [SerializeField] private float updateFrequency = 10f;
    private new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        fertilization = BaseFertilization;


        InvokeRepeating("InokeUpdate", 5f, updateFrequency);
    }

    public void Initialize(GameObject plant)
    {
        plantPrefab = plant;
    }

    private void Plant()
    {
        for(int i = 0; i < soulTrees.Count; i++)
        {
            if (soulTrees[i] == null)
            {
                Vector3 plantPosition = GetRandomPlaceOnTile();
                soulTrees[i] = Instantiate(plantPrefab, plantPosition + transform.position, Quaternion.identity).GetComponent<SoulTree>();
                soulTrees[i].transform.SetParent(transform, true);
                soulTrees[i].Initialize();
            }
        }
    }

    private Vector3 GetRandomPlaceOnTile()
    {
        Vector2 randomInSphere = UnityEngine.Random.insideUnitSphere * transform.localScale.x;
       
        return new Vector3(randomInSphere.x, 0f, randomInSphere.y);
    }

    public void Fertilize(float amount)
    {
        fertilization += amount;
    }

    protected void InokeUpdate()
    {
        Plant();
        FertilizeTrees();
    }

    private void FertilizeTrees()
    {
        if (fertilization > 50f)
        {
            foreach (SoulTree soulTree in soulTrees)
            {
                if (soulTree != null)
                {
                    soulTree.Nutritiousness += fertilization;
                }
            }
        }
    }
}
