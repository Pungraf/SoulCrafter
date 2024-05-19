using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilePlane : MonoBehaviour
{
    [SerializeField] private float fertilizeTime = 20;
    [SerializeField] private float BaseFertilization = 0.4f;

    [SerializeField] private int fertilizeMaxQuantity = 5;
    [SerializeField] GameObject plantPrefab;

    [SerializeField] private float fertilization;
    [SerializeField] private float fertilizeCounter;
    private new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        fertilization = BaseFertilization;
        fertilizeCounter = fertilizeTime;
        Ticker.Tick_05 += Update_Tick05;

        Plant();
    }

    // Update is called once per frame
    void Update()
    {
        fertilizeCounter -= Time.deltaTime;
    }

    private void Update_Tick05(object sender, EventArgs e)
    {
        if (fertilizeCounter <= 0)
        {
            Plant();
        }
    }

    public void Initialize(GameObject plant, float fertilizeTime, int FertilizeQuantity)
    {
        plantPrefab = plant;
        this.fertilizeTime = fertilizeTime;
        fertilizeMaxQuantity = FertilizeQuantity;
    }

    private void Plant()
    {
        if (WorldManager.Instance.rand.NextDouble() < fertilization)
        {
            int plantQuantity = UnityEngine.Random.Range(1, fertilizeMaxQuantity);
            for (int i = 0; i < plantQuantity; i++)
            {
                int x = (int)renderer.bounds.size.x;
                int z = (int)renderer.bounds.size.z;
                Vector3 spawnPosition = new Vector3(transform.position.x - (x / 2) + UnityEngine.Random.Range(0, x), 0f, transform.position.z - (z / 2) + UnityEngine.Random.Range(0, z));
                Food newPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity).GetComponent<Food>();
                newPlant.Initialize();
                newPlant.transform.parent = transform;
                fertilization = BaseFertilization;
            }
        }

        fertilizeCounter = fertilizeTime;
    }

    public void Fertilize(float amount)
    {
        fertilization += amount;
        fertilizeCounter /= 2;
    }
}
