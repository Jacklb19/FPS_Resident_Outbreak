using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public string weaponName = "Pistola";
    public int damage = 25;
    public float fireRate = 0.5f; // Tiempo entre disparos
    public float range = 100f;

    [Header("Munición")]
    public int maxAmmo = 30; // Balas por cargador
    public int currentAmmo;
    public int reserveAmmo = 90; // Munición de reserva
    public float reloadTime = 2f;

    [Header("Referencias")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash; // Lo crearás después con VFX

    private float nextFireTime = 0f;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading)
            return;

        // Disparo con clic izquierdo
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }

        // Recarga con R
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        // Auto-recarga si se queda sin balas
        if (currentAmmo <= 0 && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }
    void Shoot()
    {
        if (currentAmmo <= 0)
            return;

        currentAmmo--;

        // Raycast para detectar impacto
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Impacto en: " + hit.transform.name);

            // Aquí detectaremos enemigos más adelante
            // Por ahora solo visualiza el impacto

            // Aplicar daño si es un enemigo (lo haremos después)
            // EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(damage);
            // }
        }

        // Efecto visual del disparo (opcional por ahora)
        if (muzzleFlash != null)
            muzzleFlash.Play();
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recargando...");

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log("Recarga completa. Munición: " + currentAmmo);
    }

}
