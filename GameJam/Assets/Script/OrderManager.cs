using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public Queue<string> pendingOrders = new Queue<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddOrder(string foodType)
    {
        pendingOrders.Enqueue(foodType);
        Debug.Log("Order added: " + foodType);
    }
}
