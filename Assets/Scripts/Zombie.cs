using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header("Config")]
    public ZombieConfig config;

    [Header("Rangos (derivados del SO)")]
    public float detectionRange;
    public float attackRange;
    public float loseInterestDistance;

    [Header("Ataque (derivado del SO)")]
    public int attackDamage;
    public float attackCooldown;

    [Header("Sistema")]
    public bool isFromPool = false;

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

        detectionRange = config.detectionRange;
        attackRange = config.attackRange;
        loseInterestDistance = Mathf.Max(config.loseInterestDistance, detectionRange);
        attackDamage = config.attackDamage;
        attackCooldown = config.attackCooldown;
        pursuitSpeed = config.pursuitSpeed;

        if (zombieHealth != null)
        {
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

        navAgent.enabled = false;
        StartCoroutine(EnableNavMeshAfterDelay());
    }

    IEnumerator EnableNavMeshAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (navAgent != null)
        {
            navAgent.enabled = true;
        }
    }

    void Update()
    {
        if (isDead || target == null || navAgent == null || !navAgent.isOnNavMesh) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

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

        if (playerDetected && distanceToTarget > loseInterestDistance)
        {
            playerDetected = false;
            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.ResetPath();
                navAgent.isStopped = true;
            }
            if (animator != null) animator.SetBool("isWalking", false);
            return;
        }


        if (zombieHealth != null && zombieHealth.CurrentHealth <= 0)
        {
            Die();
            return;
        }

        if (distanceToTarget <= attackRange)
        {
            navAgent.isStopped = true;
            if (animator != null) animator.SetBool("isWalking", false);

            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
                StartCoroutine(PerformAttack());
        }
        else
        {
            navAgent.isStopped = false;
            if (navAgent.isOnNavMesh)
            {
                navAgent.SetDestination(target.position);
            }
            if (animator != null) animator.SetBool("isWalking", true);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (zombieHealth != null) zombieHealth.TakeDamage(amount);
        Debug.Log("Zombie DAMAGE trigger called");
        if (!isDead && animator != null) animator.SetTrigger("DAMAGE");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        isAttacking = false;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
            navAgent.enabled = false;
        }

        if (animator != null)
        {
            int randomDeath = Random.Range(0, 2);
            if (randomDeath == 0) animator.SetTrigger("DIE1");
            else animator.SetTrigger("DIE2");
        }

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddZombieKill();
        }

        enabled = false;


        if (!isFromPool)
        {
            StartCoroutine(DestroyAfterDelay(3f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }



    public void ResetZombie()
    {
        isDead = false;
        isAttacking = false;
        playerDetected = false;
        lastAttackTime = 0f;

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = false;
            navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
        }

        var col = GetComponent<Collider>();
        if (col) col.enabled = true;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
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