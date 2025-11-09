using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public int attackDamage = 10;
    public float attackRange = 2f;
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
        if (isDead) return;

        if (target == null)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        if (zombieHealth == null || zombieHealth.CurrentHealth <= 0)
        {
            Die();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

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

        zombieHealth.TakeDamage(amount); // ✅ LLAMAS A ZombieHealth

        animator.SetTrigger("DAMAGE");

        if (zombieHealth.CurrentHealth <= 0)
        {
            Die();
        }
    }

    // ✅ Muerte
    void Die()
    {
        isDead = true;
        navAgent.isStopped = true;

        animator.SetBool("isWalking", false);
        animator.SetTrigger("DIE");

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
