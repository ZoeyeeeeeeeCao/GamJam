using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Seat assignedSeat;
    private bool isSitting = false;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void AssignSeat(Seat seat)
    {
        assignedSeat = seat;

        // Move to seat position
        agent.SetDestination(seat.transform.position);
    }

    private void Update()
    {
        if (assignedSeat == null || isSitting)
            return;

        // Check if reached seat
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            SitDown();
        }
    }

    private void SitDown()
    {
        isSitting = true;
        agent.isStopped = true;

        // Rotate to match seat orientation
        transform.rotation = assignedSeat.transform.rotation;

        // Play sitting animation (if you have one)
        if (animator != null)
            animator.SetTrigger("Sit");

        Debug.Log("Customer is now sitting and waiting.");
    }

    public void LeaveRestaurant()
    {
        // Free seat
        assignedSeat.isOccupied = false;

        // Destroy customer
        Destroy(gameObject);
    }
}
