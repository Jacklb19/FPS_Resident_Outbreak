using UnityEngine;

public class ZombieProjectile : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 1f;   // se destruye tras 3s

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo reaccionar con el Player
        if (other.CompareTag("Player"))
        {
            // Busca el PlayerHealth en el objeto o sus padres
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // Si choca con algo sólido (no trigger), se destruye también
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
