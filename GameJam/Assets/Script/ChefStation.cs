using UnityEngine;
using System.Collections.Generic;

public class ChefStation : MonoBehaviour
{
    [Header("Cooking Task Setup")]
    public GameObject cookingTaskPrefab;
    public Transform cookingTaskHolder;

    [Header("Food Icons")]
    public Sprite soupIcon;
    public Sprite chickenIcon;
    public Sprite burgerIcon;

    [Header("Cooking Times")]
    public float soupTime = 3f;
    public float chickenTime = 5f;
    public float burgerTime = 4f;

    private List<FoodCookingTask> activeTasks = new List<FoodCookingTask>();

    private float nextOrderCheckTime = 0f;
    private float checkDelay = 0.5f;

    private void Update()
    {
        if (Time.time >= nextOrderCheckTime)
        {
            TryStartNextOrder();
            nextOrderCheckTime = Time.time + checkDelay;
        }
    }

    void TryStartNextOrder()
    {
        if (OrderManager.Instance.pendingOrders.Count == 0)
            return;

        string nextFood = OrderManager.Instance.pendingOrders.Dequeue();

        GameObject uiGO = Instantiate(cookingTaskPrefab, cookingTaskHolder);
        FoodCookingTask task = uiGO.GetComponent<FoodCookingTask>();
        task.foodType = nextFood;

        // Apply correct food icon
        switch (nextFood)
        {
            case "Soup":
                task.foodIcon.sprite = soupIcon;
                task.cookingTime = soupTime;
                break;

            case "Chicken":
                task.foodIcon.sprite = chickenIcon;
                task.cookingTime = chickenTime;
                break;

            case "Burger":
                task.foodIcon.sprite = burgerIcon;
                task.cookingTime = burgerTime;
                break;
        }

        activeTasks.Add(task);
        RefreshUIPositions();

        task.BeginCooking(this);
    }

    public void OnCookingFinished(string foodType, FoodCookingTask task)
    {
        activeTasks.Remove(task);
        RefreshUIPositions();

        // TODO: here we call WindowCounter later
        Debug.Log(foodType + " finished cooking!");
    }

    void RefreshUIPositions()
    {
        for (int i = 0; i < activeTasks.Count; i++)
        {
            RectTransform rt = activeTasks[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * 140f, 0);
        }
    }
}
