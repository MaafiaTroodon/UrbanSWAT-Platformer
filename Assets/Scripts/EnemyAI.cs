using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Movement")]
    public float detectRange = 20f;
    public float attackRange = 1.7f;
    public float moveSpeed = 2.8f;
    public float turnSpeed = 12f;

    [Tooltip("World m/s that maps to Animator Speed=2 (like your PlayerControl)")]
    public float runSpeedForBlend = 7f;

    [Header("Leash (stay in your zone)")]
    public float leashRadius = 6f;        // how far from spawn it is allowed to move
    public bool returnHome = true;        // walk back to home when player leaves zone
    Vector3 homePos;                      // spawn position
    float homeY;                          // keep this Y

    [Header("Animation (optional)")]
    public Animator anim;
    public bool alwaysAnimate = true;

    [Header("Attack")]
    public float attackWindup = 0.12f;
    public float extraGapBetweenSwings = 0.35f;

    EnemySwordMelee weapon;
    Rigidbody rb;
    float nextAttackTime;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;

        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            // lock rotation so it doesn't tip over
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        if (!anim) anim = GetComponentInChildren<Animator>(true);
        if (anim)
        {
            anim.applyRootMotion = false;
            if (alwaysAnimate) anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        weapon = GetComponentInChildren<EnemySwordMelee>(true);

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) player = p.transform;
        }

        homePos = transform.position;
        homeY = homePos.y;
    }

    void Update()
    {
        if (!player) return;

        // Keep Y locked to spawn height
        if (rb && rb.isKinematic) rb.MovePosition(new Vector3(transform.position.x, homeY, transform.position.z));
        else                      transform.position = new Vector3(transform.position.x, homeY, transform.position.z);

        Vector3 toPlayer = player.position - transform.position; toPlayer.y = 0f;
        float distToPlayer = toPlayer.magnitude;
        bool canSee = distToPlayer <= detectRange;

        // Compute where we are relative to HOME
        Vector3 fromHome = transform.position - homePos; fromHome.y = 0f;
        float distFromHome = fromHome.magnitude;

        // Desired move direction (toward player if inside leash, else toward leash edge/home)
        Vector3 desiredDir = Vector3.zero;
        bool inAttack = false;

        if (canSee)
        {
            // Clamp the player's chase point to the leash circle
            Vector3 toPlayerFromHome = (player.position - homePos); toPlayerFromHome.y = 0f;
            Vector3 targetWithinLeash = homePos + Vector3.ClampMagnitude(toPlayerFromHome, leashRadius);

            Vector3 toTarget = targetWithinLeash - transform.position; toTarget.y = 0f;

            // Are we close enough to attack?
            inAttack = (Vector3.Distance(transform.position, targetWithinLeash) <= attackRange + 0.05f);

            // Move toward the clamped target only if not already close
            if (!inAttack && toTarget.sqrMagnitude > 0.0001f)
                desiredDir = toTarget.normalized;

            // Face the clamped target (keeps him at the edge when player is outside)
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(toTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
            }
        }

        // If we can’t see player OR player is outside leash and returnHome is true → go home
        bool playerOutsideLeash = (player ? (player.position - homePos).sqrMagnitude > leashRadius * leashRadius : false);
        if ((!canSee || (playerOutsideLeash && returnHome)) && distFromHome > 0.05f)
        {
            Vector3 toHome = (homePos - transform.position); toHome.y = 0f;
            desiredDir = toHome.normalized;

            if (toHome.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(toHome);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
            }

            inAttack = false; // don’t attack while returning home
        }

        // Apply movement
        float speedThisFrame = 0f;
        if (desiredDir != Vector3.zero)
        {
            Vector3 step = desiredDir * (moveSpeed * Time.deltaTime);
            Vector3 next = transform.position + step;

            // Ensure next position stays within the leash circle
            Vector3 nextFromHome = next - homePos; nextFromHome.y = 0f;
            if (nextFromHome.magnitude > leashRadius)
            {
                nextFromHome = nextFromHome.normalized * leashRadius;
                next = homePos + nextFromHome;
                next.y = homeY;
            }

            if (rb && rb.isKinematic) rb.MovePosition(next);
            else                      transform.position = next;

            speedThisFrame = moveSpeed;
        }

        // Animator Speed 0..2
        if (anim)
        {
            float blend = Mathf.Clamp01(speedThisFrame / Mathf.Max(0.01f, runSpeedForBlend)) * 2f;
            anim.SetFloat("Speed", blend);
        }

        // Attack if allowed and inside leash edge
        if (canSee && !playerOutsideLeash && inAttack && weapon != null && Time.time >= nextAttackTime)
        {
            if (anim) anim.SetTrigger("Attack");
            weapon.Attack(attackWindup);
            float cool = Mathf.Max(weapon.cooldown, attackWindup + 0.05f) + extraGapBetweenSwings;
            nextAttackTime = Time.time + cool;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.15f);
        Gizmos.DrawWireSphere(Application.isPlaying ? GetHome() : transform.position, leashRadius);
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    Vector3 GetHome() => homePos;
#endif
}
