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
        GameObject prefabToSpawn = GetPrefab(type);
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab set for " + type);
            return;
        }

        
        if (currentFoods.Count >= foodSlots.Length)
        {
            Debug.LogWarning("WindowCounter is full! Can't spawn more food.");
            return;
        }

        Transform slot = foodSlots[currentFoods.Count];

        GameObject foodObj = Instantiate(prefabToSpawn, slot.position, slot.rotation);
        foodObj.transform.SetParent(slot);

        var item = foodObj.GetComponent<FoodItem>();
        if (item == null)
            item = foodObj.AddComponent<FoodItem>();

        item.foodPrefabId = prefabToSpawn;

        // ⭐⭐ 关键：告诉食物“你是这个窗口生的”
        item.ownerWindow = this;

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
            // 1. 从列表里移除
            currentFoods.Remove(foodObj);

            // 2. 不要 Destroy，让这份食物继续存在（会被玩家拿在手上）
            // Destroy(foodObj);   // ← 把这一行删掉！

            // 3. 重新整理剩下食物的位置（左移）
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
