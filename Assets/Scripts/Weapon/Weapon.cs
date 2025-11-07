using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
    private WeaponInstance weaponInstance;
    
    private float nextShotTime = 0f;
    private bool isReloading = false;
    private float lastEmptyClickTime = 0f;
    private float emptyClickCooldown = 0.3f;
    
    private AudioSource audioSource;
    private Animator weaponAnimator;
    
    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");
    private static readonly int ReloadTrigger = Animator.StringToHash("Reload");
    private static readonly int IsReloadingBool = Animator.StringToHash("IsReloading");
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        weaponAnimator = GetComponentInChildren<Animator>();
        if (weaponAnimator != null)
        {
            weaponAnimator.enabled = false;
        }
    }
    
    public void Initialize(WeaponInstance instance)
    {
        weaponInstance = instance;
        weaponData = instance.weaponData;
    }
    
    public void OnEquip()
    {
        if (weaponData != null)
        {
            transform.localPosition = weaponData.spawnPosition;
            transform.localRotation = Quaternion.Euler(weaponData.spawnRotation);
        }
        
        if (weaponAnimator != null)
        {
            weaponAnimator.enabled = true;
            weaponAnimator.Rebind();
            weaponAnimator.Update(0f);
        }
    }
    
    public void OnUnequip()
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.enabled = false;
        }
        
        if (isReloading)
        {
            StopAllCoroutines();
            isReloading = false;
        }
    }
    
    public bool TryShoot()
    {
        if (isReloading || Time.time < nextShotTime)
            return false;
        
        if (!weaponInstance.CanShoot())
        {
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
        if (weaponAnimator != null && weaponAnimator.enabled)
        {
            weaponAnimator.SetTrigger(ShootTrigger);
        }
        
        PlaySound(weaponData.shootSound);
        
        Camera mainCam = Camera.main;
        if (mainCam == null) return;
        
        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            Vector3 direction = mainCam.transform.forward;
            
            direction.x += Random.Range(-weaponData.spread, weaponData.spread);
            direction.y += Random.Range(-weaponData.spread, weaponData.spread);
            direction.Normalize();
            
            Ray ray = new Ray(mainCam.transform.position, direction);
            
            if (Physics.Raycast(ray, out RaycastHit hit, weaponData.raycastDistance, weaponData.hitMask))
            {
                if (weaponData.impactPrefab != null)
                {
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    rotation *= Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                    Vector3 spawnPosition = hit.point + hit.normal * 0.01f;
                    
                    GameObject impact = Instantiate(weaponData.impactPrefab, spawnPosition, rotation);
                    Destroy(impact, 2f);
                }
                
                PlaySound(weaponData.impactSound);
                
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(weaponData.damage);
                    
                    GameUI gameUI = FindObjectOfType<GameUI>();
                    if (gameUI != null)
                        gameUI.OnCrosshairHit();
                }
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
        
        if (weaponAnimator != null && weaponAnimator.enabled)
        {
            weaponAnimator.SetTrigger(ReloadTrigger);
            weaponAnimator.SetBool(IsReloadingBool, true);
        }
        
        PlaySound(weaponData.reloadSound);
        
        yield return new WaitForSeconds(weaponData.reloadTime);
        
        weaponInstance.Reload();
        isReloading = false;
        
        if (weaponAnimator != null && weaponAnimator.enabled)
        {
            weaponAnimator.SetBool(IsReloadingBool, false);
        }
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
