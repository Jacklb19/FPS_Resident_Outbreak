using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    public Vector3 spawnPosition = Vector3.zero;
    public Vector3 spawnRotation = Vector3.zero;
    
    public bool isActiveWeapon = false;
    public Animator animator;
    
    private int currentAmmo;
    private int totalAmmo;
    private float lastShootTime;
    private float lastEmptySoundTime;
    private float emptySoundCooldown = 0.5f;
    private bool isReloading;
    private AudioSource audioSource;
    private bool hasInitialized = false;
    
    private void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (!hasInitialized && weaponData != null)
        {
            Initialize();
            hasInitialized = true;
        }
    }
    
    private void Initialize()
    {
        currentAmmo = weaponData.magazineSize;
        totalAmmo = weaponData.magazineSize * 3;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.playOnAwake = false;
    }
    
    private void Start()
    {
        if (!hasInitialized && weaponData != null)
        {
            Initialize();
            hasInitialized = true;
        }
    }
    
    private void Update()
    {
        if (!isActiveWeapon)
            return;
        
        if (Input.GetMouseButton(0))
        {
            Fire();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
    
    public void Fire()
    {
        if (weaponData == null || isReloading || currentAmmo <= 0)
            return;
        
        if (Time.time - lastShootTime < weaponData.shootingDelay)
            return;
        
        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            FireHitscan();
        }
        
        if (animator != null)
            animator.SetTrigger("Recoil");
        
        if (audioSource != null && weaponData.shootSound != null)
            audioSource.PlayOneShot(weaponData.shootSound);
        
        currentAmmo--;
        lastShootTime = Time.time;
    }
    
    private void FireHitscan()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;
        
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, weaponData.raycastDistance, weaponData.hitMask))
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponData.damage, hit.point, hit.normal);
            }
        }
    }
    
    public void Reload()
    {
        if (isReloading || currentAmmo == weaponData.magazineSize || totalAmmo <= 0)
            return;
        
        StartCoroutine(ReloadCoroutine());
    }
    
    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        
        if (animator != null)
            animator.SetTrigger("Reload");
        
        if (audioSource != null && weaponData.reloadSound != null)
            audioSource.PlayOneShot(weaponData.reloadSound);
        
        yield return new WaitForSeconds(weaponData.reloadTime);
        
        int bulletosNecesarios = weaponData.magazineSize - currentAmmo;
        
        if (totalAmmo >= bulletosNecesarios)
        {
            currentAmmo = weaponData.magazineSize;
            totalAmmo -= bulletosNecesarios;
        }
        else
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }
        
        isReloading = false;
    }
    
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMagazineSize() => weaponData.magazineSize;
    public int GetTotalAmmo() => totalAmmo;
    public string GetWeaponName() => weaponData.weaponName;
}
