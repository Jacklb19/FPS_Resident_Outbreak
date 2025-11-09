using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 10f; // Distancia para empezar a perseguir
    public float attackRange = 2f;

    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    [Header("Movimiento")]
    public float pursuitSpeed = 5.5f;

    private float lastAttackTime = 0f;
    private Transform target;
    private Animator animator;
    private NavMeshAgent navAgent;
    private ZombieHealth zombieHealth;

    private bool isAttacking = false;
    private bool isDead = false;
    private bool playerDetected = false; // ✅ Nuevo estado

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        zombieHealth = GetComponent<ZombieHealth>();

        navAgent.speed = pursuitSpeed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // ✅ Detectar jugador solo si está en rango
        if (!playerDetected)
        {
            if (distanceToTarget <= detectionRange)
            {
                playerDetected = true;
                Debug.Log("Zombie: ¡Jugador detectado!");
            }
            else
            {
                animator.SetBool("isWalking", false); // Idle o caminando lento
                return;
            }
        }

        // ✅ Si está detectado, ya persigue.
        if (zombieHealth.CurrentHealth <= 0)
        {
            Die();
            return;
        }

        if (distanceToTarget <= attackRange)
        {
            navAgent.isStopped = true;
            animator.SetBool("isWalking", false);

            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartCoroutine(PerformAttack());
            }
        }
        else
        {
            navAgent.isStopped = false;
            animator.SetBool("isWalking", true);
            navAgent.SetDestination(target.position);
        }
    }

    // ✅ Recibir daño
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        zombieHealth.TakeDamage(amount);

        if (!isDead)
            animator.SetTrigger("DAMAGE");
    }

    // ✅ Muerte aleatoria
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        navAgent.isStopped = true;
        animator.SetBool("isWalking", false);

        int randomDeath = Random.Range(0, 2);

        if (randomDeath == 0)
            animator.SetTrigger("DIE1");
        else
            animator.SetTrigger("DIE2");

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, 5f);
    }

    System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;

        animator.SetTrigger("DAMAGE");

        yield return new WaitForSeconds(0.5f);

        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        lastAttackTime = Time.time;

        yield return new WaitForSeconds(1f);
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
