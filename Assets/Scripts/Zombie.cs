using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;
    private NavMeshAgent navAgent;
    public Transform target;

    void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            navAgent.SetDestination(target.position);
        }

        // Control de animación de caminar
        if (navAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        Debug.Log("Daño recibido: " + damageAmount + " | HP actual: " + HP);

        if (HP <= 0)
        {
            // Elegir aleatoriamente entre las dos animaciones de muerte
            int dieAnim = Random.Range(0, 2);
            if (dieAnim == 0)
            {
                animator.SetTrigger("DIE1");
                Debug.Log("Trigger DIE1 activado");
            }
            else
            {
                animator.SetTrigger("DIE2");
                Debug.Log("Trigger DIE2 activado");
            }

            Debug.Log("Zombi destruido");
            Destroy(gameObject, 2f); // Espera 2 segundos para ver la animación de muerte antes de destruirlo
        }
        else
        {
            animator.SetTrigger("DAMAGE");
            Debug.Log("Trigger DAMAGE activado | HP actual: " + HP);
        }
    }

}
