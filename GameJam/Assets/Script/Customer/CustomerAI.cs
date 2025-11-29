using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Seat seat;

    private bool isSitting = false;

    public Transform exitPoint; //spawnpoint, which is the restaurant exit!!

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void AssignSeat(Seat targetSeat)
    {
        seat = targetSeat;

        // Walk towards seat
        agent.SetDestination(seat.transform.position);

        if (animator != null)
            animator.SetBool("IsMoving", true);
    }

    private void Update()
    {
        if (seat == null || isSitting)
            return;

        if (!agent.pathPending && agent.remainingDistance <= 0.1f)
        {
            SitInstantly();
        }
    }

    private void SitInstantly()
    {
        isSitting = true;
        agent.isStopped = true;
        agent.ResetPath();

        // Snap position to exact seat location
        transform.position = seat.transform.position;

        // Snap rotation to seat rotation
        transform.rotation = seat.transform.rotation;

        // Switch to sitting animation
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsSitting", true);
        }

        Debug.Log("Customer is now sitting (instant sit).");
    }

    // Later you'll use this to make them leave:
    public void StandAndLeave()
    {
        isSitting = false;

        if (animator != null)
            animator.SetBool("IsSitting", false);

        seat.isOccupied = false;
        Destroy(gameObject);
    }

    public void LeaveRestaurant()
    {
        Debug.Log("Customer leaving restaurant");

        if (agent == null) return;

        // Customer is no longer sitting
        isSitting = false;

        // Reset seat occupation
        if (seat != null)
            seat.isOccupied = false;

        // Stop sitting animation and play walk animation
        if (animator != null)
        {
            animator.SetBool("IsSitting", false);
            animator.SetBool("IsMoving", true);
        }

        // Allow movement again
        agent.isStopped = false;

        // Walk toward exit point
        if (exitPoint != null)
        {
            agent.SetDestination(exitPoint.position);
            Debug.Log("Customer walking toward exit: " + exitPoint.position);
        }
        else
        {
            Debug.LogWarning("EXIT POINT IS NULL!");
        }

        StartCoroutine(DestroyAfterReachingExit());
    }


    private IEnumerator DestroyAfterReachingExit()
    {
        // Wait until path becomes valid
        while (agent.pathPending)
            yield return null;

        // Wait until agent moves (remainingDistance updates)
        while (agent.remainingDistance == Mathf.Infinity)
            yield return null;

        // Wait until customer reaches the exit
        while (agent.remainingDistance > 0.2f)
            yield return null;

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }

}
