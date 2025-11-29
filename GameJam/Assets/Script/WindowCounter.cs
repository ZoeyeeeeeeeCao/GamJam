using System.Collections.Generic;
using UnityEngine;

public class WindowCounter : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] foodSlots;      // Size 2 in Inspector

    [Header("Food Prefabs")]
    public GameObject soupPrefab;
    public GameObject chickenPrefab;
    public GameObject burgerPrefab;

    private List<GameObject> currentFoods = new List<GameObject>();

    public bool IsFull()
    {
        return currentFoods.Count >= foodSlots.Length;
    }

    public void SpawnFinishedFood(FoodType type)
    {
        if (IsFull())
        {
            Debug.LogWarning("WindowCounter is full! Can't spawn more food.");
            return;
        }

        GameObject prefabToSpawn = GetPrefab(type);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab set for " + type);
            return;
        }

        Transform slot = foodSlots[currentFoods.Count];

        GameObject foodObj = Instantiate(prefabToSpawn, slot.position, slot.rotation);
        foodObj.transform.SetParent(slot);

        currentFoods.Add(foodObj);
    }

    private GameObject GetPrefab(FoodType type)
    {
        switch (type)
        {
            case FoodType.Soup: return soupPrefab;
            case FoodType.Chicken: return chickenPrefab;
            case FoodType.Burger: return burgerPrefab;
        }
        return null;
    }

    // Optional: call this from your "pickup" script later
    public void RemoveFood(GameObject foodObj)
    {
        if (currentFoods.Contains(foodObj))
        {
            currentFoods.Remove(foodObj);
            Destroy(foodObj);

            // Re-pack foods into slots so they shift left visually
            RepackSlots();
        }
    }

    private void RepackSlots()
    {
        for (int i = 0; i < currentFoods.Count; i++)
        {
            GameObject f = currentFoods[i];
            Transform slot = foodSlots[i];
            f.transform.position = slot.position;
            f.transform.rotation = slot.rotation;
            f.transform.SetParent(slot);
        }
    }
}
