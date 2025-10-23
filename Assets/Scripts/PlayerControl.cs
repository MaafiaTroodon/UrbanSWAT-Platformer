using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;              // walk speed (XZ only)
    public float sprintSpeed = 12f;           // sprint speed (XZ only)
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float jumpForce = 7f;              // vertical impulse
    public float turnSpeed = 12f;             // rotate toward move dir

    [Header("References")]
    public Transform cam;                     // Main Camera transform
    public LayerMask groundMask = ~0;         // ground layers
    public Animator anim;                     // Animator on the SWAT child

    [Header("Ground Check")]
    public float groundCheckDistance = 1.1f;  // ray length below player

    // Animator blend normalisation (Speed 0..2 for Idle/Walk/Run)
    [Tooltip("World m/s that maps to Speed=2 in the blend tree (use your sprint speed).")]
    public float runSpeedForBlend = 12f;

    [Header("Camera FOV Kick")]
    public float fovWalk = 60f;
    public float fovSprint = 74f;
    public float fovLerpSpeed = 6f;

    Rigidbody rb;
    float h, v;
    bool jumpPressed;
    bool isGrounded;
    bool sprinting;

    Camera camComponent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        // keep upright
        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (!anim) anim = GetComponentInChildren<Animator>();
        if (anim) anim.applyRootMotion = false;

        if (!cam) cam = Camera.main ? Camera.main.transform : null;
        camComponent = cam ? cam.GetComponent<Camera>() : Camera.main;
        if (camComponent) camComponent.fieldOfView = fovWalk;

        // Make sure the blend mapping lines up by default
        if (runSpeedForBlend <= 0f) runSpeedForBlend = sprintSpeed;
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        sprinting = Input.GetKey(sprintKey);

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
            if (anim) anim.SetTrigger("Jump");
        }

        // FOV kick (smooth)
        if (camComponent)
        {
            float targetFov = (sprinting && isGrounded) ? fovSprint : fovWalk;
            camComponent.fieldOfView = Mathf.Lerp(
                camComponent.fieldOfView, targetFov,
                Time.unscaledDeltaTime * fovLerpSpeed
            );
        }
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down,
                                     groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

        // Camera-relative move dir (XZ)
        Vector3 f = cam ? cam.forward : Vector3.forward; f.y = 0f; f.Normalize();
        Vector3 r = cam ? cam.right   : Vector3.right;   r.y = 0f; r.Normalize();
        Vector3 dir = (f * v + r * h).normalized;

        // Velocity (walk vs sprint)
        float targetSpeed = (sprinting && isGrounded) ? sprintSpeed : moveSpeed;
        Vector3 vel = dir * targetSpeed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;

        // Smooth face move direction
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            Quaternion newRot = Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            rb.angularVelocity = Vector3.zero;
            rb.MoveRotation(newRot);
        }

        // Jump
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
        jumpPressed = false;

        // Animator parameters (Speed 0..2 in your blend tree)
        if (anim)
        {
            float flatSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
            float blend = Mathf.Clamp01(flatSpeed / Mathf.Max(0.01f, runSpeedForBlend)) * 2f;
            anim.SetFloat("Speed", blend);
            anim.SetBool("Grounded", isGrounded);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
#endif
}
