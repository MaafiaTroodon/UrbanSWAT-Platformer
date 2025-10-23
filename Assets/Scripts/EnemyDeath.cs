using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDeath : MonoBehaviour
{
    public Animator anim;
    public float destroyDelay = 2f;

    void Awake()
    {
        GetComponent<Health>().onDeath.AddListener(Die);
    }

    void Die()
    {
        // stop interacting
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        var rb = GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;

        if (anim) anim.SetTrigger("Die"); // if you have a death anim
        Destroy(gameObject, destroyDelay);
    }
}
