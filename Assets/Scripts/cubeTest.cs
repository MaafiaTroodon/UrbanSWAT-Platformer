using UnityEngine;

public class cubeTest : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        rend = GetComponent<Renderer>();
        if (rend) originalColor = rend.material.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage! HP left: {currentHealth}");

        // Small hit flash
        if (rend)
        {
            rend.material.color = Color.red;
            Invoke(nameof(ResetColor), 0.2f);
        }

        if (currentHealth <= 0)
            Die();
    }

    void ResetColor()
    {
        if (rend) rend.material.color = originalColor;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject);
    }
}
