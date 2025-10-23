using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onHealed;
    public UnityEvent onDeath;

    public bool IsDead => currentHP <= 0;

    float invulnerableUntil = 0f;

    void Awake()
    {
        currentHP = Mathf.Max(1, maxHP);
    }

    public void SetMax(int newMax, bool refill = true)
    {
        maxHP = Mathf.Max(1, newMax);
        currentHP = Mathf.Clamp(refill ? maxHP : currentHP, 0, maxHP);
        onHealed?.Invoke();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        int before = currentHP;
        currentHP = Mathf.Clamp(currentHP + Mathf.Abs(amount), 0, maxHP);
        if (currentHP > before) onHealed?.Invoke();
    }

    public void MakeInvulnerable(float seconds)
    {
        invulnerableUntil = Mathf.Max(invulnerableUntil, Time.time + Mathf.Max(0f, seconds));
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (Time.time < invulnerableUntil) return;

        currentHP = Mathf.Clamp(currentHP - Mathf.Abs(amount), 0, maxHP);
        Debug.Log($"{name} HP = {currentHP}");

        onDamaged?.Invoke();
        if (currentHP == 0) onDeath?.Invoke();
    }

    public void ResetToMax()
    {
        currentHP = maxHP;
        onHealed?.Invoke();
    }
}
