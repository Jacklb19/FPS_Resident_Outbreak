using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    private WeaponInstance weaponInstance;
    
    private float nextShotTime = 0f;
    private bool isReloading = false;
    
    // ═══ NUEVO: Cooldown para sonido de arma vacía ═══
    private float lastEmptyClickTime = 0f;
    private float emptyClickCooldown = 0.3f; // Tiempo entre sonidos de arma vacía
    
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    public void Initialize(WeaponInstance instance)
    {
        weaponInstance = instance;
        weaponData = instance.weaponData;
    }
    
    public bool TryShoot()
    {
        if (isReloading || Time.time < nextShotTime)
            return false;
        
        if (!weaponInstance.CanShoot())
        {
            // ═══ SOLUCIÓN: Solo reproducir si ha pasado el cooldown ═══
            if (Time.time >= lastEmptyClickTime + emptyClickCooldown)
            {
                PlaySound(weaponData.emptyMagazineSound);
                lastEmptyClickTime = Time.time;
            }
            return false;
        }
        
        PerformShot();
        weaponInstance.ConsumeAmmo();
        nextShotTime = Time.time + weaponData.shootingDelay;
        
        return true;
    }
    
    private void PerformShot()
    {
        PlaySound(weaponData.shootSound);
        
        Camera mainCam = Camera.main;
        if (mainCam == null) return;
        
        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            Vector3 direction = mainCam.transform.forward;
            
            // Aplicar spread
            direction.x += Random.Range(-weaponData.spread, weaponData.spread);
            direction.y += Random.Range(-weaponData.spread, weaponData.spread);
            direction.Normalize();
            
            Ray ray = new Ray(mainCam.transform.position, direction);
            
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.raycastDistance, weaponData.hitMask))
            {
                // Impacto visual
                if (weaponData.impactPrefab != null)
                {
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    rotation *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                    Vector3 spawnPosition = hit.point + hit.normal * 0.01f;
                    
                    GameObject impact = Instantiate(weaponData.impactPrefab, spawnPosition, rotation);
                    Destroy(impact, 2f);
                }
                
                PlaySound(weaponData.impactSound);
                
                // Daño a enemigos
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(weaponData.damage);
                    
                    GameUI gameUI = FindObjectOfType<GameUI>();
                    if (gameUI != null)
                        gameUI.OnCrosshairHit();
                }
                
                Debug.DrawLine(ray.origin, hit.point, Color.red, 0.5f);
            }
        }
        
        GameUI ui = FindObjectOfType<GameUI>();
        if (ui != null)
            ui.OnCrosshairShot();
    }
    
    public void StartReload()
    {
        if (!weaponInstance.CanReload() || isReloading)
            return;
        
        StartCoroutine(ReloadCoroutine());
    }
    
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        PlaySound(weaponData.reloadSound);
        
        yield return new WaitForSeconds(weaponData.reloadTime);
        
        weaponInstance.Reload();
        isReloading = false;
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public int GetCurrentAmmo() => weaponInstance.currentMagazineAmmo;
    public int GetTotalAmmo() => weaponInstance.totalReserveAmmo;
    public bool IsReloading() => isReloading;
    
    public WeaponInstance GetWeaponInstance() => weaponInstance;
}
