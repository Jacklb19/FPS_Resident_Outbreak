using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public string weaponName = "Pistola";
    public int damage = 25;
    public float fireRate = 0.5f;
    public float range = 100f;

    [Header("Munición")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;
    public float reloadTime = 2f;

    [Header("Referencias")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public Transform muzzlePoint; // Punto donde sale el disparo

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound; // Sonido cuando no hay balas
    private AudioSource audioSource;

    [Header("Efectos")]
    public GameObject impactEffect; // Efecto de impacto en superficies
    public float muzzleFlashDuration = 0.1f;

    private float nextFireTime = 0f;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;

        // Obtener o agregar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar AudioSource
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound para FPS
    }

    void Update()
    {
        if (isReloading)
            return;

        // Disparo con clic izquierdo
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
            {
                // Sonido de arma vacía
                PlayEmptySound();
            }
        }

        // Recarga con R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        // Auto-recarga si se queda sin balas al intentar disparar
        if (Input.GetButtonDown("Fire1") && currentAmmo <= 0 && reserveAmmo > 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
            return;

        currentAmmo--;

        // Reproducir sonido de disparo
        PlayShootSound();

        // Efecto visual del disparo
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();

            // Agregar luz temporal
            StartCoroutine(MuzzleFlashLight());
        }

        // Raycast para detectar impacto
        RaycastHit hit;
        Vector3 rayOrigin = fpsCam.transform.position;
        Vector3 rayDirection = fpsCam.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, range))
        {
            Debug.Log("Impacto en: " + hit.transform.name);

            // Crear efecto de impacto en el punto de colisión
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            // Detectar si es un zombie y aplicar daño
            ZombieHealth zombie = hit.transform.GetComponent<ZombieHealth>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                Debug.Log("Daño aplicado al zombie: " + damage);
            }

            // También verificar en el padre (por si el collider está en un hijo)
            if (zombie == null)
            {
                zombie = hit.transform.GetComponentInParent<ZombieHealth>();
                if (zombie != null)
                {
                    zombie.TakeDamage(damage);
                    Debug.Log("Daño aplicado al zombie (desde padre): " + damage);
                }
            }
        }

        // Visualizar el rayo en el editor (solo para debug)
        Debug.DrawRay(rayOrigin, rayDirection * range, Color.red, 1f);
    }

    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;
        Debug.Log("Recargando...");

        // Reproducir sonido de recarga
        PlayReloadSound();

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log("Recarga completa. Munición actual: " + currentAmmo + " | Reserva: " + reserveAmmo);
    }

    // Funciones de audio
    void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    void PlayReloadSound()
    {
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }

    void PlayEmptySound()
    {
        if (audioSource != null && emptySound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(emptySound);
        }
    }

    // Función para obtener munición (cuando el jugador recoge más balas)
    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log("Munición añadida. Reserva actual: " + reserveAmmo);
    }
    IEnumerator MuzzleFlashLight()
{
    // Crear una luz temporal en la punta del cañón
    GameObject lightObj = new GameObject("TempLight");
    lightObj.transform.SetParent(muzzleFlash.transform);
    lightObj.transform.localPosition = Vector3.zero;
    
    Light tempLight = lightObj.AddComponent<Light>();
    tempLight.type = LightType.Point;
    tempLight.intensity = 500f;
    tempLight.range = 15f;
    tempLight.color = new Color(1f, 0.8f, 0.5f); // Naranja
    
    yield return new WaitForSeconds(0.1f);
    
    Destroy(lightObj);
}

}
