using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public float speed = 2f;                    // units per second along the path

    Rigidbody rb;
    float pathLen;                               // |B - A|
    float s;                                     // 0..1 position along the path
    int dir = 1;                                 // +1 forward, -1 back

    void Awake()
    {
        // Ensure we have a rigidbody set up for stable moving-platform collisions
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Make sure our collider is NOT a MeshCollider or at least not non-convex
        // Prefer a BoxCollider on the platform root/child that matches the top surface.
    }

    void Start()
    {
        if (!a || !b) return;

        // Compute total path length
        pathLen = Vector3.Distance(a.position, b.position);

        // Initialize 's' from current world position so we DON'T snap to A
        if (pathLen > 0.0001f)
        {
            Vector3 ap = transform.position - a.position;
            Vector3 ab = (b.position - a.position);
            float t = Vector3.Dot(ap, ab) / ab.sqrMagnitude; // projection 0..1 (clamped)
            s = Mathf.Clamp01(t);
        }
        else
        {
            s = 0f;
        }
    }

    void FixedUpdate()
    {
        if (!a || !b || pathLen < 0.0001f) return;

        // Advance along the path at constant speed, bounce at the ends
        s += (speed / pathLen) * dir * Time.fixedDeltaTime;
        if (s >= 1f) { s = 1f; dir = -1; }
        if (s <= 0f) { s = 0f; dir =  1; }

        Vector3 target = Vector3.Lerp(a.position, b.position, s);
        rb.MovePosition(target);
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.CompareTag("Player"))
            c.transform.SetParent(transform);
    }

    void OnCollisionExit(Collision c)
    {
        if (c.transform.CompareTag("Player"))
            c.transform.SetParent(null);
    }
}
