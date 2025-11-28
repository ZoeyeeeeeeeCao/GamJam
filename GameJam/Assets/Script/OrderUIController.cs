using UnityEngine;

public class OrderUIController : MonoBehaviour
{
    public GameObject orderPanel;

    private void Start()
    {
        orderPanel.SetActive(false);
    }

    public void OpenOrderUI()
    {
        orderPanel.SetActive(true);
    }

    public void CloseOrderUI()
    {
        orderPanel.SetActive(false);
    }

    public void OrderSoup()
    {
        OrderManager.Instance.AddOrder("Soup");
        CloseOrderUI();
    }

    public void OrderChicken()
    {
        OrderManager.Instance.AddOrder("Chicken");
        CloseOrderUI();
    }

    public void OrderBurger()
    {
        OrderManager.Instance.AddOrder("Burger");
        CloseOrderUI();
    }
}
