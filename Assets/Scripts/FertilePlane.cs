using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilePlane : MonoBehaviour
{
    [SerializeField] private float fertilizeTime = 20;
    [SerializeField] private int fertilizeMaxQuantity = 3;
    [SerializeField] GameObject plantPrefab;

    [SerializeField] private float fertilizeCounter;
    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        fertilizeCounter = fertilizeTime;
    }

    // Update is called once per frame
    void Update()
    {
        fertilizeCounter -= Time.deltaTime;

        if(fertilizeCounter <= 0)
        {
            int plantQuantity = Random.Range(1, fertilizeMaxQuantity);
            for(int i = 0; i < fertilizeMaxQuantity; i++)
            {
                int x = (int)renderer.bounds.size.x;
                int z = (int)renderer.bounds.size.z;
                Vector3 spawnPosition = new Vector3(transform.position.x - (x/2) + Random.Range(0, x), 0f, transform.position.z - (z / 2) + Random.Range(0, z));
                GameObject newPlant = Instantiate(plantPrefab, spawnPosition, Quaternion.identity);
                newPlant.transform.parent = transform;
            }
            fertilizeCounter = fertilizeTime;
        }
    }
}
