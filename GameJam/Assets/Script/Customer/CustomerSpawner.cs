using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public float spawnInterval = 4f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            TrySpawnCustomer();
            timer = 0f;
        }
    }

    private void TrySpawnCustomer()
    {
        Seat freeSeat = SeatManager.Instance.GetFirstAvailableSeat();

        if (freeSeat == null)
        {
            Debug.Log("No seat available — customer not spawned.");
            return;
        }

        // Spawn customer
        GameObject customerObj = Instantiate(customerPrefab, transform.position, Quaternion.identity);

        // Tell customer which seat to walk to
        CustomerAI customer = customerObj.GetComponent<CustomerAI>();
        customer.AssignSeat(freeSeat);

        // Mark seat as occupied
        freeSeat.isOccupied = true;
    }
}
