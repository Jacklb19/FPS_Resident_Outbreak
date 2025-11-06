using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public WeaponData weaponData;

    private int currentAmmo;
    private int totalAmmo;
    private float lastShootTime;
    private float lastEmptySoundTime;
    private float emptySoundCooldown = 0.5f;
    private bool isReloading;
    private bool hasBeenInitialized;
    private Animator animator;
    private AudioSource audioSource;
    private Transform bulletSpawnPoint;

    private void OnEnable()
    {
        if (!hasBeenInitialized && weaponData != null)
        {
            Initialize();
            hasBeenInitialized = true;
        }
    }

    public void Initialize()
    {
        if (weaponData == null)
            return;

        currentAmmo = weaponData.magazineSize;
        totalAmmo = weaponData.magazineSize * 3;
        lastShootTime = 0f;
        isReloading = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        bulletSpawnPoint = transform.Find("BulletSpawn");
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = gameObject.AddComponent<Animator>();
    }

    public void Fire()
    {
        if (weaponData == null || isReloading)
            return;

        if (currentAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }

        if (Time.time - lastShootTime < weaponData.shootingDelay)
            return;

        for (int i = 0; i < weaponData.bulletsPerShot; i++)
        {
            FireHitscan();
        }

        TriggerRecoilAnimation();
        PlayShootSound();

        GameUI gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
            gameUI.OnCrosshairShot();

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
            SpawnImpact(hit.point, hit.normal);

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(weaponData.damage, hit.point, hit.normal);

                GameUI gameUI = FindObjectOfType<GameUI>();
                if (gameUI != null)
                    gameUI.OnCrosshairHit();
            }

            PlayImpactSound(hit.point);
        }
    }

    private void SpawnImpact(Vector3 position, Vector3 normal)
    {
        if (weaponData.impactPrefab == null)
            return;

        GameObject impactInstance = Instantiate(weaponData.impactPrefab, position, Quaternion.identity);

        Impact impactScript = impactInstance.GetComponent<Impact>();
        if (impactScript != null)
            impactScript.SetRotation(normal);

        Destroy(impactInstance, 1f);
    }

    private void TriggerRecoilAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Recoil");
    }

    private void PlayShootSound()
    {
        if (weaponData.shootSound != null && audioSource != null)
            audioSource.PlayOneShot(weaponData.shootSound);
    }

    private void PlayEmptySound()
    {
        if (weaponData.emptyMagazineSound == null || audioSource == null)
            return;

        if (Time.time - lastEmptySoundTime >= emptySoundCooldown)
        {
            audioSource.PlayOneShot(weaponData.emptyMagazineSound);
            lastEmptySoundTime = Time.time;
        }
    }

    private void PlayImpactSound(Vector3 hitPoint)
    {
        if (weaponData.impactSound != null)
            AudioSource.PlayClipAtPoint(weaponData.impactSound, hitPoint, 0.7f);
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == weaponData.magazineSize)
            return;

        if (totalAmmo <= 0)
        {
            PlayEmptySound();
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        if (animator != null)
            animator.SetTrigger("Reload");

        if (audioSource != null && weaponData.reloadSound != null)
            audioSource.PlayOneShot(weaponData.reloadSound);

        yield return new WaitForSeconds(weaponData.reloadTime);

        int bullasNecesarias = weaponData.magazineSize - currentAmmo;

        if (totalAmmo >= bullasNecesarias)
        {
            currentAmmo = weaponData.magazineSize;
            totalAmmo -= bullasNecesarias;
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
    public bool IsReloading() => isReloading;
    public WeaponData GetWeaponData() => weaponData;
}
