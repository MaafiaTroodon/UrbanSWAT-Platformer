using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    [Header("Platform Points")]
    public Transform pointA;      // starting point (lower)
    public Transform pointB;      // ending point (upper)

    [Header("Movement Settings")]
    public float speed = 2f;      // speed of movement
    public bool pingPong = true;  // move back and forth

    [Header("Carry Player")]
    public bool carryPlayer = true;

    private Rigidbody rb;
    private Vector3 targetPos;

    void Start()
    {
        if (!pointA || !pointB)
        {
            Debug.LogError("⚠️ VerticalPlatform: Please assign Point A and Point B!");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        targetPos = pointB.position; // start going up
    }

    void FixedUpdate()
    {
        if (!pointA || !pointB) return;

        // Move platform smoothly
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Check if reached the target (using distance tolerance)
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            if (pingPong)
            {
                // toggle target between A and B
                targetPos = (Vector3.Distance(targetPos, pointA.position) < 0.1f)
                    ? pointB.position
                    : pointA.position;
            }
        }
    }

    // Stick player to platform
    void OnCollisionEnter(Collision col)
    {
        if (carryPlayer && col.collider.CompareTag("Player"))
            col.collider.transform.SetParent(transform);
    }

    // Unstick player when leaving
    void OnCollisionExit(Collision col)
    {
        if (carryPlayer && col.collider.CompareTag("Player"))
            col.collider.transform.SetParent(null);
    }
}
