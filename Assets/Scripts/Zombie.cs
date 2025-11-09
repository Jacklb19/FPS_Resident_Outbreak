using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Zombie : MonoBehaviour
{
    [Header("Rangos")]
    public float detectionRange = 10f; // rango para detectar y activar chase/rage
    public float attackRange = 2f;

    [Header("Velocidades")]
    public float pursuitSpeed = 2.0f; // velocidad normal para caminar/perseguir
    public float rageSpeed = 4.5f;    // velocidad al detectar (correr)

    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;

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

        if (navAgent != null)
        {
            navAgent.speed = pursuitSpeed;
            navAgent.acceleration = 8f;
            navAgent.angularSpeed = 120f;
            navAgent.updateRotation = true;
            navAgent.updatePosition = true;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Detecta entrada / salida del rango de detección
        if (!playerDetected && distanceToTarget <= detectionRange)
        {
            playerDetected = true;
            OnPlayerDetected();
        }
        else if (playerDetected && distanceToTarget > detectionRange)
        {
            playerDetected = false;
            OnPlayerLost();
        }

        // Si no detectó, puede estar idle o caminar leve (aquí lo dejamos quieto)
        if (!playerDetected)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            return;
        }

        // Si está muerto, sale
        if (zombieHealth != null && zombieHealth.CurrentHealth <= 0)
        {
            Die();
            return;
        }

        // Si está en rango de ataque
        if (distanceToTarget <= attackRange)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);

            // Rotar hacia el jugador
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartCoroutine(PerformAttack());
            }
        }
        else // perseguir
        {
            navAgent.isStopped = false;
            animator.SetBool("isWalking", true);

            // Si está en modo rage, activa run anim
            animator.SetBool("isRunning", playerDetected);
            navAgent.speed = playerDetected ? rageSpeed : pursuitSpeed;

            navAgent.SetDestination(target.position);
        }
    }

    // Llamado cuando detecta player
    private void OnPlayerDetected()
    {
        Debug.Log("Zombie: Jugador detectado -> RAGE!");
        if (animator != null)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
        }
        if (navAgent != null) navAgent.speed = rageSpeed;
        // aquí podrías reproducir un sonido o efecto
    }

    // Llamado cuando pierde al player
    private void OnPlayerLost()
    {
        Debug.Log("Zombie: Perdí al jugador, vuelvo a patrullar/idle");
        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
        }
        if (navAgent != null) navAgent.speed = pursuitSpeed;
    }

    // Recibir daño
    public void TakeDamage(int amount)
    {
        if (isDead) return;
        if (zombieHealth != null)
        {
            zombieHealth.TakeDamage(amount);
            if (!isDead && animator != null) animator.SetTrigger("DAMAGE");
        }
    }

    // Muerte
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (navAgent != null) { navAgent.isStopped = true; navAgent.velocity = Vector3.zero; }
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            int randomDeath = Random.Range(0, 2);
            if (randomDeath == 0) animator.SetTrigger("DIE1");
            else animator.SetTrigger("DIE2");
        }

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, 5f);
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        if (animator != null) animator.SetTrigger("ATTACK");

        yield return new WaitForSeconds(0.4f); // sincronizar con tu anim

        if (target != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(attackDamage);
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.6f);
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
