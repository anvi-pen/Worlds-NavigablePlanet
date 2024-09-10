using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFruits : MonoBehaviour
{
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private Transform fruitsParent;

    // Start is called before the first frame update
    void Start()
    {
        int fruitsToSpawn = 100;
        int numPlants = transform.childCount;
        int step = numPlants / fruitsToSpawn;
        int start = Random.Range(0, step);

        for (int i = start; i < numPlants; i = i + step)
        {
            Transform curPlant = transform.GetChild(i);
            GameObject fruit = Instantiate(fruitPrefab, curPlant.position, curPlant.rotation, fruitsParent);
            fruit.transform.Translate(fruit.transform.up * 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
