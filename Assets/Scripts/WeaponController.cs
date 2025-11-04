using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public string weaponName = "Pistola";
    public int damage = 25;
    public float fireRate = 0.5f;
    public float range = 100f;
    public Sprite weaponIcon;

    [Header("Munición")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;
    public float reloadTime = 2f;

    [Header("Referencias")]
    public Camera fpsCam;
    public ParticleSystem shootParticles;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    public AudioClip hitSound;
    private AudioSource audioSource;

    [Header("Efectos")]
    public GameObject impactEffect;
    public GameObject bulletHoleDecal;
    public float bulletHoleLifetime = 30f;

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
        audioSource.spatialBlend = 0f;
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
                PlayEmptySound();
            }
        }

        // Recarga con R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        // Auto-recarga si se queda sin balas
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

        // Efecto de muzzle flash
        if (shootParticles != null)
            shootParticles.Play();

        // Raycast para detectar impacto
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Impacto en: " + hit.transform.name);

            // Reproducir sonido de impacto
            if (hitSound != null && audioSource != null)
                audioSource.PlayOneShot(hitSound);

            // Crear efecto de impacto visual
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            // Crear hueco de bala
            if (bulletHoleDecal != null)
            {
                CreateBulletHole(hit);
            }

            // Detectar si es un zombie y aplicar daño
            ZombieHealth zombie = hit.transform.GetComponent<ZombieHealth>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
                Debug.Log("Daño aplicado al zombie: " + damage);
                return;
            }

            // Verificar en el padre
            if (zombie == null)
            {
                zombie = hit.transform.GetComponentInParent<ZombieHealth>();
                if (zombie != null)
                {
                    zombie.TakeDamage(damage);
                    Debug.Log("Daño aplicado al zombie (desde padre): " + damage);
                    return;
                }
            }

            // Aplicar fuerza si tiene rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(-hit.normal * 5f, ForceMode.Impulse);
        }

        // Debug ray
        Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * range, Color.red, 1f);
    }

    void CreateBulletHole(RaycastHit hit)
    {
        // Crear el decal ligeramente separado de la superficie
        Vector3 spawnPosition = hit.point + hit.normal * 0.01f;
        
        GameObject hole = Instantiate(
            bulletHoleDecal,
            spawnPosition,
            Quaternion.LookRotation(hit.normal)
        );

        // Hacer hijo del objeto impactado
        hole.transform.SetParent(hit.transform);

        // Rotación aleatoria
        hole.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));

        // Destruir después del tiempo
        Destroy(hole, bulletHoleLifetime);
    }

    IEnumerator Reload()
    {
        if (isReloading)
            yield break;

        isReloading = true;
        Debug.Log("Recargando...");

        PlayReloadSound();

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log("Recarga completa. Munición actual: " + currentAmmo + " | Reserva: " + reserveAmmo);
    }

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

    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
        Debug.Log("Munición añadida. Reserva actual: " + reserveAmmo);
    }
}
