using UnityEngine;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    private int currentHealth;
    
    public int CurrentHealth => currentHealth;
    
    private Animator animator;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        Debug.Log($"Zombie recibió {damage} de daño. Salud restante: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Opcional: reproducir animación de hit
            if (animator != null)
            {
                // Si tienes un trigger para hit, úsalo aquí
                // animator.SetTrigger("Hit");
            }
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        Debug.Log("Zombie murió");
        
        // Desactivar componentes
        Zombie zombieScript = GetComponent<Zombie>();
        if (zombieScript != null)
        {
            zombieScript.enabled = false;
        }
        
        UnityEngine.AI.NavMeshAgent navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.enabled = false;
        }
        
        // Desactivar colisiones
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("DIE");
        }
        
        // Destruir después de un tiempo
        Destroy(gameObject, 5f);
    }
}
