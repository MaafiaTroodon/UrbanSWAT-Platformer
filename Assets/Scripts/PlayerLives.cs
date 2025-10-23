using UnityEngine;
using UnityEngine.UI;

public class PlayerLives : MonoBehaviour
{
    [Header("References")]
    public Health health;                 // Player's Health on the root
    public Rigidbody rb;                  // Player rigidbody
    public CapsuleCollider bodyCollider;  // main collider (recommended)
    public PlayerControl playerControl;   // movement script (optional)

    [Header("Lives")]
    public int startingLives = 3;
    public Image[] lifeIcons;

    [Header("Respawn Rules")]
    public bool respawnAtDeathSpot = true;
    public Transform baseSpawn;
    public LayerMask groundMask = ~0;
    public float groundSnapDist = 6f;     // how far below we search for a floor
    public float respawnInvulnSeconds = 1.0f;
    public float groundOffset = 0.06f;    // tiny lift to avoid intersecting surface

    [Header("Game Over")]
    public bool reloadSceneOnGameOver = false;
    public float reloadDelay = 1.5f;

    int lives;
    bool handlingDeath = false;
    Vector3 lastDeathPos;
    Quaternion lastDeathRot;

    void Awake()
    {
        if (!health) health = GetComponent<Health>();
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!bodyCollider) bodyCollider = GetComponentInChildren<CapsuleCollider>();
        if (!playerControl) playerControl = GetComponent<PlayerControl>();

        lives = Mathf.Max(1, startingLives);
        UpdateIcons();

        if (health) health.onDeath.AddListener(OnDeath);
    }

    void OnDestroy()
    {
        if (health) health.onDeath.RemoveListener(OnDeath);
    }

    void OnDeath()
    {
        if (handlingDeath) return;
        handlingDeath = true;

        lastDeathPos = transform.position;
        lastDeathRot = transform.rotation;

        lives--;
        UpdateIcons();

        if (lives > 0)
        {
            if (respawnAtDeathSpot)
                RespawnAt(SnapToGroundNear(lastDeathPos), lastDeathRot);
            else
                RespawnAt(SnapToGroundNear(BasePoint()), Quaternion.identity);
        }
        else
        {
            if (reloadSceneOnGameOver)
            {
                if (playerControl) playerControl.enabled = false;
                Invoke(nameof(ReloadActiveScene), reloadDelay);
            }
            else
            {
                // soft “base” respawn + refill lives
                RespawnAt(SnapToGroundNear(BasePoint()), Quaternion.identity);
                lives = startingLives;
                UpdateIcons();
            }
        }

        handlingDeath = false;
    }

    // ---- Strong, collider-aware respawn placement (prevents falling through) ----
    void RespawnAt(Vector3 pos, Quaternion rot)
    {
        // temporarily disable player-driven movement
        if (playerControl) playerControl.enabled = false;

        // turn physics off while we teleport
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // place transform (slightly above ground)
        Vector3 snapPos = pos + Vector3.up * groundOffset;
        transform.SetPositionAndRotation(snapPos, rot);

        // sync transforms so colliders update immediately
        Physics.SyncTransforms();

        // wake + re-enable physics
        if (rb)
        {
            rb.isKinematic = false;
            rb.WakeUp();
        }

        // tiny extra lift next fixed frame then re-enable control
        StartCoroutine(ReenableControlNextFixed());

        // refill health & give brief invulnerability
        if (health)
        {
            health.ResetToMax();
            health.MakeInvulnerable(respawnInvulnSeconds);
        }
    }

    System.Collections.IEnumerator ReenableControlNextFixed()
    {
        yield return new WaitForFixedUpdate();
        if (rb) rb.position += Vector3.up * 0.001f; // microsafe nudge
        if (playerControl) playerControl.enabled = true;
    }

    Vector3 BasePoint() => baseSpawn ? baseSpawn.position : transform.position;

    // Find the ground below/near a point using the player capsule size when available
    Vector3 SnapToGroundNear(Vector3 from)
    {
        // If we have a capsule collider, use a capsule cast (more reliable than a ray)
        if (bodyCollider)
        {
            float radius = Mathf.Max(0.01f, bodyCollider.radius * Mathf.Abs(transform.lossyScale.x));
            float height = Mathf.Max(radius * 2f, bodyCollider.height * Mathf.Abs(transform.lossyScale.y));
            float half = height * 0.5f;

            Vector3 up = Vector3.up;
            Vector3 center = transform.TransformPoint(bodyCollider.center);
            Vector3 p1 = center + up * (half - radius + 0.01f);
            Vector3 p2 = center - up * (half - radius + 0.01f);

            // start a little above the requested point
            Vector3 originOffset = up * 0.5f;
            p1 = p1 + (from - center) + originOffset;
            p2 = p2 + (from - center) + originOffset;

            if (Physics.CapsuleCast(p1, p2, radius, Vector3.down, out RaycastHit hit,
                                    groundSnapDist + 1f, groundMask, QueryTriggerInteraction.Ignore))
            {
                return hit.point;
            }
        }
        else
        {
            // fallback: simple raycast
            Vector3 origin = from + Vector3.up * 0.5f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit,
                                groundSnapDist + 1f, groundMask, QueryTriggerInteraction.Ignore))
                return hit.point;
        }

        // nothing found; just use the provided point
        return from;
    }

    void UpdateIcons()
    {
        if (lifeIcons == null) return;
        for (int i = 0; i < lifeIcons.Length; i++)
            if (lifeIcons[i]) lifeIcons[i].gameObject.SetActive(i < lives);
    }

    void ReloadActiveScene()
    {
        var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(s.buildIndex);
    }

    // optional helper you added earlier—kept here for completeness
    public void KillAllLivesNow()
    {
        lives = 0;
        UpdateIcons();

        if (health) health.ResetToMax();

        if (reloadSceneOnGameOver)
        {
            if (playerControl) playerControl.enabled = false;
            Invoke(nameof(ReloadActiveScene), reloadDelay);
        }
        else
        {
            RespawnAt(SnapToGroundNear(BasePoint()), Quaternion.identity);
            lives = startingLives;
            UpdateIcons();
        }
    }
}
