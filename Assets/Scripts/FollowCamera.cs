using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Targets")]
    public Transform target;           // Player (Transform)
    public Rigidbody targetRb;         // Player's Rigidbody (optional but recommended)

    [Header("Framing")]
    public Vector3 offset = new Vector3(0f, 4.5f, -6f); // height + distance behind

    [Header("Responsiveness")]
    [Tooltip("Lower = tighter follow. 0.02–0.06 is typical.")]
    public float followDampTime = 0.03f;
    [Tooltip("Max yaw speed (deg/sec) the camera is allowed to rotate.")]
    public float maxYawSpeed = 540f;
    [Tooltip("Velocity needed (m/s) before we consider it 'moving forward'.")]
    public float minForwardSpeed = 0.35f;
    [Tooltip("How strongly sideways (strafe) motion can influence camera yaw (0 = ignore).")]
    [Range(0f, 1f)] public float lateralInfluence = 0.1f;

    // internal
    Vector3 followVel;                  // for SmoothDamp
    Vector3 facing = Vector3.forward;   // cached yaw-only forward

    void LateUpdate()
    {
        if (!target) return;

        // --- 1) Decide which way "forward" should be (XZ only), ignoring most strafe ---
        Vector3 desiredFwd = transform.forward; // fallback = keep current facing

        if (targetRb)
        {
            // Horizontal velocity only
            Vector3 v = targetRb.velocity; v.y = 0f;

            if (v.sqrMagnitude > minForwardSpeed * minForwardSpeed)
            {
                // Split velocity into forward (along current cam forward) and lateral
                Vector3 camF = transform.forward; camF.y = 0f; camF.Normalize();
                Vector3 vForward = Vector3.Project(v, camF);
                Vector3 vLateral = v - vForward;

                // Let lateral motion influence yaw only a little
                Vector3 blended = vForward + vLateral * lateralInfluence;

                if (blended.sqrMagnitude > 0.0001f)
                    desiredFwd = blended.normalized;
            }
        }

        // Smoothly rotate camera's yaw toward desiredFwd, with a HARD speed cap
        Vector3 currentF = transform.forward; currentF.y = 0f; currentF.Normalize();
        Vector3 nextF = Vector3.RotateTowards(currentF, desiredFwd,
                        Mathf.Deg2Rad * maxYawSpeed * Time.deltaTime, 1e9f);
        facing = nextF;

        Quaternion yaw = Quaternion.LookRotation(facing, Vector3.up);

        // --- 2) Follow position (no tilt): tight but smooth ---
        // use rb.position if available (works with Rigidbody interpolation)
        Vector3 targetPos = targetRb ? targetRb.position : target.position;
        Vector3 desiredPos = targetPos + yaw * offset;

        // Smooth XZ with SmoothDamp, but filter Y separately to remove bob
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos,
                                                ref followVel, followDampTime);

        // Extra Y-only smoothing (very small, just to kill micro jitter)
        transform.position = new Vector3(
            transform.position.x,
            Mathf.Lerp(transform.position.y, desiredPos.y, 0.15f),  // 0.10–0.25 = light filter
            transform.position.z
        );


        // --- 3) Look at the player, keep upright, capped turn rate (no “dips”) ---
        Vector3 lookPoint = target.position + Vector3.up * 1.2f;
        Quaternion lookRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot,
                                                      maxYawSpeed * Time.deltaTime);
    }
}
