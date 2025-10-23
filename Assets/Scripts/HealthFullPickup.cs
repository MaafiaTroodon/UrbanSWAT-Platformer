using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthFullPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public bool onlyAffectsPlayer = true;        // if true, requires PlayerControl component
    public float invulnerableSeconds = 0.5f;     // brief grace after heal
    public bool destroyOnPickup = true;          // remove pickup after use

    [Header("FX (optional)")]
    public AudioClip sfx;
    public GameObject vfxPrefab;

    void Reset()
    {
        // ensure trigger is set
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // find a Health on the thing that touched us
        var health = other.GetComponentInParent<Health>();
        if (!health) return;

        // optionally ensure it's the player (not enemies)
        if (onlyAffectsPlayer && !other.GetComponentInParent<PlayerControl>())
            return;

        // if already full, you can still play FX and consume; or early-out:
        if (health.currentHP < health.maxHP)
        {
            health.ResetToMax();                           // <- refill current life HP
            if (invulnerableSeconds > 0f)
                health.MakeInvulnerable(invulnerableSeconds);
        }

        // FX
        if (vfxPrefab) Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        if (sfx) AudioSource.PlayClipAtPoint(sfx, transform.position);

        if (destroyOnPickup) Destroy(gameObject);
    }
}
