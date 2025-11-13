using UnityEngine;
using System;

public class BossHealth : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    public int maxHealth = 800;
    private int currentHealth;

    public event Action OnBossDied;
    public event Action<int, int> OnDamaged; // (current, damage)

    private bool isDead = false;

    void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int dmg = Mathf.Max(0, damage);
        currentHealth = Mathf.Max(0, currentHealth - dmg);

        Debug.Log($"[Boss] Recibió {dmg} daño. Vida: {currentHealth}/{maxHealth}");
        OnDamaged?.Invoke(currentHealth, dmg);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[Boss] Derrotado");
        OnBossDied?.Invoke();

        // Animación de muerte, efectos, etc.
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("DIE");
        }

        Destroy(gameObject, 5f);
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
}
