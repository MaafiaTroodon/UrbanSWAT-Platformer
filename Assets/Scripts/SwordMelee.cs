using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwordMelee : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 25;
    public LayerMask hitMask;                  // set to Enemy only

    [Header("Hitbox (local space)")]
    public Vector3 boxSize = new Vector3(1f, 4f, 1f);
    public Vector3 boxCenter = new Vector3(0f, 2f, 0.5f);

    [Header("Attack Timing")]
    public float activeTime = 0.15f;
    public float cooldown = 0.35f;

    [Header("Animation + Effects (Optional)")]
    public Animator anim;
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

        // Force box hitbox dimensions if it is a BoxCollider
        if (hitbox is BoxCollider bc)
        {
            bc.size = boxSize;
            bc.center = boxCenter;
        }

        if (!anim) anim = GetComponentInParent<Animator>();
        if (slashTrail) slashTrail.emitting = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
            StartCoroutine(Slash());
    }

    IEnumerator Slash()
    {
        canAttack = false;
        active = true;

        if (anim && !string.IsNullOrEmpty(animTrigger)) anim.SetTrigger(animTrigger);
        if (swingAudio) swingAudio.Play();

        hitbox.enabled = true;
        if (slashTrail) slashTrail.emitting = true;

        yield return new WaitForSeconds(activeTime);

        hitbox.enabled = false;
        if (slashTrail) slashTrail.emitting = false;
        active = false;

        yield return new WaitForSeconds(Mathf.Max(0, cooldown - activeTime));
        canAttack = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!active) return; // only deal damage while attack window is open
        if (((1 << other.gameObject.layer) & hitMask) == 0) return; // respect Enemy mask

        Health target = other.GetComponentInParent<Health>();
        if (target)
        {
            target.TakeDamage(damage);
            Debug.Log($"🩸 Sword hit {other.name}, dealt {damage}.");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // visualize the box in the Scene view
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Matrix4x4 m = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = m;
    }
#endif
}
