using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header("Config")]
    public ZombieConfig config; // Asigna desde el inspector

    [Header("Rangos (derivados del SO)")]
    public float detectionRange;
    public float attackRange;
    public float loseInterestDistance;

    [Header("Ataque (derivado del SO)")]
    public int attackDamage;
    public float attackCooldown;

    [Header("Movimiento (derivado del SO)")]
    public float pursuitSpeed;

    private float lastAttackTime = 0f;
    private Transform target;
    private Animator animator;
    private NavMeshAgent navAgent;
    private ZombieHealth zombieHealth;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool playerDetected = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        zombieHealth = GetComponent<ZombieHealth>();

        // Validaciones
        if (config == null)
        {
            Debug.LogError($"ZombieConfig no asignado en {gameObject.name}!", this);
            enabled = false;
            return;
        }
        if (navAgent == null)
        {
            Debug.LogError($"NavMeshAgent faltante en {gameObject.name}!", this);
            enabled = false;
            return;
        }

        // Cargar parámetros desde el SO
        detectionRange = config.detectionRange;
        attackRange = config.attackRange;
        loseInterestDistance = Mathf.Max(config.loseInterestDistance, detectionRange);
        attackDamage = config.attackDamage;
        attackCooldown = config.attackCooldown;
        pursuitSpeed = config.pursuitSpeed;

        if (zombieHealth != null)
        {
            // Importante: usar el setter público en lugar de tocar el campo privado. [web:61]
            zombieHealth.SetMaxHealth(config.maxHealth, true);
        }

        navAgent.speed = pursuitSpeed;
        navAgent.stoppingDistance = Mathf.Clamp(attackRange * 0.6f, 0.1f, Mathf.Max(0.1f, attackRange - 0.1f));

        int characterLayer = LayerMask.NameToLayer("Character");
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (var obj in allObjects)
        {
            if (obj.layer == characterLayer && obj.GetComponent<CharacterController>() != null)
            {
                target = obj.transform;
                Debug.Log($"[Zombie] ✅ Player encontrado por layer Character: {obj.name}");
                break;
            }
        }

        if (target == null)
        {
            Debug.LogWarning($"[Zombie {gameObject.name}] ❌ No se encontró jugador en layer Character");
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Detección
        if (!playerDetected)
        {
            if (distanceToTarget <= detectionRange)
            {
                playerDetected = true;
            }
            else
            {
                if (animator != null) animator.SetBool("isWalking", false);
                return;
            }
        }

        // Pérdida de interés
        if (playerDetected && distanceToTarget > loseInterestDistance)
        {
            playerDetected = false;
            navAgent.ResetPath();
            navAgent.isStopped = true;
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }

        // Muerte
        if (zombieHealth != null && zombieHealth.CurrentHealth <= 0)
        {
            Die();
            return;
        }

        // Ataque o persecución
        if (distanceToTarget <= attackRange)
        {
            navAgent.isStopped = true;
            if (animator != null) animator.SetBool("isWalking", false);

            // Mirar hacia el jugador
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                StartCoroutine(PerformAttack());
        }
        else
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(target.position);
            if (animator != null) animator.SetBool("isWalking", true);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (zombieHealth != null) zombieHealth.TakeDamage(amount);
        if (!isDead && animator != null) animator.SetTrigger("HIT");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        isAttacking = false;

        navAgent.isStopped = true;
        navAgent.ResetPath();

        if (animator != null)
        {
            int randomDeath = Random.Range(0, 2);
            if (randomDeath == 0) animator.SetTrigger("DIE1");
            else animator.SetTrigger("DIE2");
        }

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // Si luego migras a pooling, reemplaza esto por devolver al pool. [web:2]
        Destroy(gameObject, 5f);
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (animator != null) animator.SetTrigger("ATTACK");

        yield return new WaitForSeconds(0.5f);

        if (isDead) yield break;

        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            var playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
