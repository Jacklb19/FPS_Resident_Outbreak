using UnityEngine;

public class Impact : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;
    private ParticleSystem particleSystem;

    private void Start()
    {
        // ✅ Buscar en el objeto y en sus hijos
        particleSystem = GetComponentInChildren<ParticleSystem>();

        if (particleSystem == null)
        {
            Debug.LogWarning($"No se encontró ParticleSystem en {gameObject.name}");
        }
        else
        {
            particleSystem.Play();
        }

        Destroy(gameObject, lifetime);
    }


    public void SetRotation(Vector3 normal)
    {
        transform.up = normal;
    }
}
