using UnityEngine;

public class OrderWindowTrigger : MonoBehaviour
{
    public OrderUIController uiController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiController.OpenOrderUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiController.CloseOrderUI();
        }
    }
}
