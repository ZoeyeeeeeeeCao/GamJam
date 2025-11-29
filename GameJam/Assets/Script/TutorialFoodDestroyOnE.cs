using UnityEngine;

public class TutorialFoodDestroyOnE : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            // 按 E 就把这堆食物 Destroy
            Destroy(gameObject);
        }
    }
}
