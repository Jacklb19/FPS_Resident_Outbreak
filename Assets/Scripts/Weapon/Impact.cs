using UnityEngine;

public class Impact : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    private ParticleSystem particleSystem;

    private void Start()
    {
        // ✅ Autodetectar Particle System
        particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem == null)
        {
            Debug.LogWarning($"No se encontró ParticleSystem en {gameObject.name}");
        }
        else
        {
            particleSystem.Play();
            Debug.Log($"Impact particles spawned at {transform.position}");
        }

        Destroy(gameObject, lifetime);
    }

    public void SetRotation(Vector3 normal)
    {
        transform.up = normal;
    }
}
