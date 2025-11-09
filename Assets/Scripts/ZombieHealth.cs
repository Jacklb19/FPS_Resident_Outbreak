using UnityEngine;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Zombie recibió {damage} de daño. Salud restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            isDead = true;

            // ✅ Notificar al script Zombie que murió
            Zombie zombieScript = GetComponent<Zombie>();
            if (zombieScript != null)
            {
                zombieScript.Die();
            }
        }
    }
}
