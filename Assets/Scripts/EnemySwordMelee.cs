using System.Collections;
using UnityEngine;

public class EnemySwordMelee : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 25;
    public LayerMask hitMask;              // set to Player layer in Inspector

    [Header("Timing")]
    public float activeTime = 0.15f;       // how long hitbox is on
    public float cooldown = 0.35f;         // time until next attack allowed

    [Header("Animation + FX (optional)")]
    public Animator anim;                  // enemy Animator
    public string animTrigger = "Attack";
    public AudioSource swingAudio;
    public TrailRenderer slashTrail;

    Collider hitbox;
    bool canAttack = true;
    bool active = false;

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        if (!hitbox) hitbox = GetComponentInChildren<Collider>();
        hitbox.isTrigger = true;
        hitbox.enabled = false;

        if (!anim) anim = GetComponentInParent<Animator>();
        if (slashTrail) slashTrail.emitting = false;
    }

    // Call this from EnemyAI when you want to attack (with optional windup)
    public void Attack(float windupSeconds = 0f)
    {
        if (canAttack) StartCoroutine(CoAttack(windupSeconds));
    }

    IEnumerator CoAttack(float windup)
    {
        canAttack = false;

        if (anim && !string.IsNullOrEmpty(animTrigger)) anim.SetTrigger(animTrigger);
        if (windup > 0f) yield return new WaitForSeconds(windup);

        if (swingAudio) swingAudio.Play();

        active = true;
        hitbox.enabled = true;
        if (slashTrail) slashTrail.emitting = true;

        yield return new WaitForSeconds(activeTime);

        hitbox.enabled = false;
        if (slashTrail) slashTrail.emitting = false;
        active = false;

        yield return new WaitForSeconds(Mathf.Max(0f, cooldown - activeTime));
        canAttack = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if (((1 << other.gameObject.layer) & hitMask) == 0) return;

        Health hp = other.GetComponentInParent<Health>();
        if (hp)
        {
            hp.TakeDamage(damage);
            Debug.Log($"🧛 Sword hit {other.name} for {damage}");
        }
    }
}
