using System;
using UnityEngine;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    // Lecturas
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    // Eventos
    public event Action<int, int> OnDamaged; // (currentHealth, damage)
    public event Action OnDied;

    void OnEnable()
    {
        // Asegura estado limpio al reactivar (ideal para pooling). [web:54]
        isDead = false;
        currentHealth = Mathf.Max(1, maxHealth);
    }

    void Start()
    {
        if (currentHealth <= 0) currentHealth = Mathf.Max(1, maxHealth);
    }

    public void SetMaxHealth(int newMax, bool fillToMax = true)
    {
        maxHealth = Mathf.Max(1, newMax);
        if (fillToMax) currentHealth = maxHealth;
        else currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + Mathf.Abs(amount), maxHealth);
        OnDamaged?.Invoke(currentHealth, 0);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        int dmg = Mathf.Max(0, damage);
        currentHealth = Mathf.Max(0, currentHealth - dmg);
        OnDamaged?.Invoke(currentHealth, dmg);

        if (currentHealth <= 0)
        {
            isDead = true;
            OnDied?.Invoke();

            var zombie = GetComponent<Zombie>();
            if (zombie != null) zombie.Die();
        }
    }
}
