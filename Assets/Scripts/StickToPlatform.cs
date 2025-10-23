using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickToPlatform : MonoBehaviour
{
    private Transform originalParent;   // original parent (usually null or enemy root)
    private Rigidbody rb;

    void Awake()
    {
        originalParent = transform.parent;
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if collided object is a moving platform
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            // Parent enemy to the platform so it moves with it
            transform.SetParent(collision.collider.transform, true);

            // Optional: prevent Y jitter if enemy uses physics
            if (rb)
                rb.interpolation = RigidbodyInterpolation.None;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("MovingPlatform"))
        {
            // Restore original parenting
            transform.SetParent(originalParent, true);

            if (rb)
                rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}
