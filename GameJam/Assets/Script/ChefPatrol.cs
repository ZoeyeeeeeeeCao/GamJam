using UnityEngine;

public class ChefPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform targetPoint;
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        targetPoint = pointB; // start walking toward B
    }

    private void Update()
    {
        // Move chef forward
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime);

        // Set walking animation
        animator.SetFloat("Speed", speed);

        // When close to target → switch direction
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;

            // Turn chef to face new direction
            transform.LookAt(targetPoint);
        }
    }
}
