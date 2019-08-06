using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{

    public GameObject[] pickUpTypes;

    int pickUpCounter = 0;
    public int numberToSpawn;

    public GameObject barrel;
    int barrelCount = 0;
    public int barrelsToSpawn;

    public float fieldHeight = 15;
    public float fieldWidth = 30;

    // Start is called before the first frame update
    void Start()
    {
        if (pickUpTypes.Length == 0)
        {
            Debug.Log("No items set in array");
        }

        if (!barrel)
        {
            Debug.Log("Barrel object not set");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pickUpCounter <= 0)
        {
            SpawnPickUps();
        }

        if (barrelCount <= 0)
        {
            SpawnBarrels();
        }
    }

    void SpawnPickUps()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-fieldWidth / 2, fieldWidth / 2), Random.Range(-fieldHeight / 2, fieldHeight / 2), 0);
            GameObject type = pickUpTypes[Random.Range(0, pickUpTypes.Length)];

            Instantiate(type, pos, transform.rotation);
            pickUpCounter++;

        }
    }

    void SpawnBarrels()
    {
        for (int i = 0; i < barrelsToSpawn; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-15, 15), Random.Range(-7.5f, 7.5f), -20);
            Instantiate(barrel, pos, transform.rotation);
            Debug.Log(message: "Position at which barrel was spawned: " + pos);
            barrelCount++;

        }
    }

    public void DecrementBarrels()
    {
        barrelCount--;
    }

    public void DecrementPickUps()
    {
        pickUpCounter--;

    }
}
