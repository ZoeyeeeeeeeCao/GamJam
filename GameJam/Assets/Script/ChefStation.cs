using System.Collections.Generic;
using UnityEngine;

public class ChefStation : MonoBehaviour
{
    [Header("Cooking Queue")]
    private Queue<FoodType> orderQueue = new Queue<FoodType>();

    [Header("Cooking UI")]
    public Transform cookingUIParent;      // Parent for the small cooking icons
    public GameObject cookingTaskUIPrefab; // Prefab with circular fill

    public float uiSpacing = 160f;         // Distance between icons

    [Header("Food Icons")]
    public Sprite soupIcon;
    public Sprite chickenIcon;
    public Sprite burgerIcon;

    [Header("Cooking Times")]
    public float soupTime = 3f;
    public float chickenTime = 5f;
    public float burgerTime = 4f;

    [Header("Counter")]
    public WindowCounter windowCounter;    // Where finished food appears

    private List<CookingTaskUI> activeTasks = new List<CookingTaskUI>();

    // Called by OrderUIController
    public void EnqueueOrder(FoodType type)
    {
        orderQueue.Enqueue(type);
        StartNextOrdersIfPossible();
    }

    // For now: we start a cooking task for every order immediately
    private void StartNextOrdersIfPossible()
    {
        while (orderQueue.Count > 0)
        {
            FoodType next = orderQueue.Dequeue();
            CreateCookingTask(next);
        }
    }

    private void CreateCookingTask(FoodType type)
    {
        GameObject go = Instantiate(cookingTaskUIPrefab, cookingUIParent);
        CookingTaskUI task = go.GetComponent<CookingTaskUI>();

        float duration = GetCookingTime(type);
        Sprite icon = GetIcon(type);

        task.Init(this, type, duration, icon);

        activeTasks.Add(task);
        RefreshTaskPositions();
    }

    private float GetCookingTime(FoodType type)
    {
        switch (type)
        {
            case FoodType.Soup: return soupTime;
            case FoodType.Chicken: return chickenTime;
            case FoodType.Burger: return burgerTime;
        }
        return 3f;
    }

    private Sprite GetIcon(FoodType type)
    {
        switch (type)
        {
            case FoodType.Soup: return soupIcon;
            case FoodType.Chicken: return chickenIcon;
            case FoodType.Burger: return burgerIcon;
        }
        return null;
    }

    // Called by CookingTaskUI when finished
    public void NotifyTaskFinished(CookingTaskUI task, FoodType type)
    {
        activeTasks.Remove(task);
        RefreshTaskPositions();

        // Spawn finished food on the window counter
        if (windowCounter != null)
        {
            windowCounter.SpawnFinishedFood(type);
        }
    }

    private void RefreshTaskPositions()
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            RectTransform rt = activeTasks[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * uiSpacing, 0f);
        }
    }
}
