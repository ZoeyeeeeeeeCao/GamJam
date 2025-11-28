using UnityEngine;
using System.Collections.Generic;

public class WindowCounter : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] foodSlots = new Transform[2];

    [Header("Food Prefabs")]
    public GameObject soupPrefab;
    public GameObject chickenPrefab;
    public GameObject burgerPrefab;

    private List<GameObject> currentFoods = new List<GameObject>();

    public bool IsFull()
    {
        return currentFoods.Count >= 2;
    }

    public void SpawnFinishedFood(string type)
    {
        if (IsFull())
        {
            Debug.LogWarning("Counter full!");
            return;
        }

        GameObject prefabToSpawn = null;
        switch (type)
        {
            case "Soup": prefabToSpawn = soupPrefab; break;
            case "Chicken": prefabToSpawn = chickenPrefab; break;
            case "Burger": prefabToSpawn = burgerPrefab; break;
        }

        Transform slot = foodSlots[currentFoods.Count];
        GameObject foodObj = Instantiate(prefabToSpawn, slot.position, slot.rotation);
        currentFoods.Add(foodObj);
    }
}
