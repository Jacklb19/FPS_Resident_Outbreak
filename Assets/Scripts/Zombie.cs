using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    [Header("Movimiento")]
    public float pursuitSpeed = 5.5f;

    protected float lastAttackTime = 0f;
    protected Transform target;
    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected ZombieHealth zombieHealth;

    protected bool isAttacking = false;
    protected bool isDead = false;
    protected bool playerDetected = false;

    protected virtual void Start()
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

        // 🔒 Mientras está atacando, no dejamos que el NavMeshAgent lo mueva
        if (isAttacking)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;      // por si tenía algo de velocidad
            animator.SetBool("isWalking", false);

            // Opcional: que siga mirando al jugador durante el ataque
            Vector3 dir = (target.position - transform.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);

            return; // no procesamos nada más de movimiento mientras ataca
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (!playerDetected)
        {
            if (distanceToTarget <= detectionRange)
            {
                playerDetected = true;
                Debug.Log("Zombie: ¡Jugador detectado!");
            }
            else
            {
                animator.SetBool("isWalking", false);
                return;
            }
        }

        if (zombieHealth.CurrentHealth <= 0)
        {
            Die();
            return;
        }

        if (distanceToTarget <= attackRange)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
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

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        zombieHealth.TakeDamage(amount);

        if (!isDead)
            animator.SetTrigger("DAMAGE");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
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

    protected virtual System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Por seguridad, paramos el agente aquí también
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;

        animator.SetTrigger("ATTACK");

        // Momento en el que el golpe "conecta"
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

        // Espera a que termine la animación antes de volver a moverse
        yield return new WaitForSeconds(0.7f);

        isAttacking = false;
        navAgent.isStopped = false;   // vuelve a perseguir
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
