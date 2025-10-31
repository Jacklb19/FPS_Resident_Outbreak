using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("Efectos")]
    public GameObject deathEffect; // Partículas o efecto al morir

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Obtener componentes
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida restante: " + currentHealth);

        // Reproducir sonido de golpe
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Trigger de animación de daño (opcional)
        if (animator != null && currentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }

        // Verificar si murió
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " ha muerto");

        // Reproducir sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Activar animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Crear efecto de muerte
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Desactivar componentes
        DisableEnemy();

        // Destruir después de un tiempo (para que se complete la animación)
        Destroy(gameObject, 3f);
    }

    void DisableEnemy()
    {
        // Desactivar el movimiento del zombie
        var navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }

        // Desactivar el collider para que no bloquee
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Desactivar scripts de comportamiento
        var enemyScripts = GetComponents<MonoBehaviour>();
        foreach (var script in enemyScripts)
        {
            if (script != this && script != animator)
            {
                script.enabled = false;
            }
        }
    }

    // Función para curar (opcional)
    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log(gameObject.name + " curado. Vida actual: " + currentHealth);
    }
}
