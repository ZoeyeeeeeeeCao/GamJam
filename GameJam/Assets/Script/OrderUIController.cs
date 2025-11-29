using UnityEngine;

public class OrderUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject orderPanel;     // Panel with Soup/Chicken/Burger buttons

    [Header("References")]
    public ChefStation chefStation;   // Drag your ChefStation here in Inspector
    public WindowCounter windowCounter;   // window counter so that u cannot make orders if the counter is full.


    private void Start()
    {
        if (orderPanel != null)
            orderPanel.SetActive(false);
    }

    public void OpenOrderUI()
    {
        if (orderPanel != null)
            orderPanel.SetActive(true);
    }

    public void CloseOrderUI()
    {
        if (orderPanel != null)
            orderPanel.SetActive(false);
    }

    // These will be called by button OnClick in the Inspector
    public void OrderSoup()
    {
        if (windowCounter.IsFull())
        {
            Debug.Log("Pick up items from counter before you can make new orders!");
            CloseOrderUI();
            return;
        }

        chefStation.EnqueueOrder(FoodType.Soup);
        CloseOrderUI();
    }


    public void OrderChicken()
    {
        if (windowCounter.IsFull())
        {
            Debug.Log("Pick up items from counter before you can make new orders!");
            CloseOrderUI();
            return;
        }

        chefStation.EnqueueOrder(FoodType.Chicken);
        CloseOrderUI();
    }


    public void OrderBurger()
    {
        if (windowCounter.IsFull())
        {
            Debug.Log("Pick up items from counter before you can make new orders!");
            CloseOrderUI();
            return;
        }

        chefStation.EnqueueOrder(FoodType.Burger);
        CloseOrderUI();
    }

}
