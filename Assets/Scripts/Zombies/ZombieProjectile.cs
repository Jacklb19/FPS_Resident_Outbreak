using UnityEngine;

public class ZombieProjectile : MonoBehaviour
{
    public float lifeTime = 3f; // Tiempo antes de autodestruirse

    private void Start()
    {
        // Se destruye solo después de 'lifeTime' segundos.
        Destroy(gameObject, lifeTime);
    }

}
