using UnityEngine;
using UnityEngine.AI;

public class ZombieAnimator : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Transform target; // El jugador o destino
    
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 5.5f;
    [SerializeField] private float stoppingDistance = 1.5f;
    [SerializeField] private float attackDistance = 2f;
    
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        // Si no hay objetivo, idle
        if (target == null)
        {
            navMeshAgent.velocity = Vector3.zero;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetFloat("Speed", 0);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Si está dentro de distancia de ataque, detente y ataca
        if (distanceToTarget <= attackDistance)
        {
            navMeshAgent.velocity = Vector3.zero;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking", true);
            return;
        }

        // Si está lejos, corre
        if (distanceToTarget > stoppingDistance + 2f)
        {
            navMeshAgent.speed = runSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.SetDestination(target.position);
            
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", false);
            animator.SetFloat("Speed", 1f);
        }
        // Si está cerca pero no en rango de ataque, camina
        else if (distanceToTarget > stoppingDistance)
        {
            navMeshAgent.speed = walkSpeed;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.SetDestination(target.position);
            
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
            animator.SetFloat("Speed", 0.5f);
        }
        else
        {
            navMeshAgent.velocity = Vector3.zero;
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        // Verificar caída
        CheckFalling();
    }

    void HandleAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Z_Attack"))
        {
            // Si la animación de ataque termina
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                animator.SetBool("IsAttacking", false);
            }
        }
    }

    void CheckFalling()
    {
        // Raycast hacia abajo para detectar suelo
        if (!Physics.Raycast(transform.position, Vector3.down, 0.5f))
        {
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Death()
    {
        isDead = true;
        animator.SetBool("IsDead", true);
        navMeshAgent.enabled = false;
    }
}
