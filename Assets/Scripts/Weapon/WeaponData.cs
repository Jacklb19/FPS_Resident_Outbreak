using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName = "New Weapon";
    public string weaponID = "weapon_001";

    [Header("Firing Settings")]
    public float shootingDelay = 0.1f;
    public int bulletsPerShot = 1;

    [Header("Physics")]
    public float bulletImpactForce = 10f;

    [Header("Spread")]
    public float hipSpread = 0.4f;
    public float adsSpread = 0.04f;
    public int damage = 25;

    [Header("Magazine Settings")]
    public int magazineSize = 30;
    public float reloadTime = 2f;

    // ═══ NUEVO: CONFIGURACIÓN DE MUNICIÓN ═══
    [Header("Ammo Settings")]
    [Tooltip("Munición inicial de reserva al recoger el arma")]
    public int startingReserveAmmo = 90; // 3 cargadores por defecto

    [Tooltip("Munición máxima de reserva que puedes llevar")]
    public int maxReserveAmmo = 210; // 7 cargadores por defecto

    public enum FireMode { Semi, Burst, Full }

    [Header("Fire Mode")]
    public FireMode fireMode = FireMode.Semi;

    [Header("Hitscan Settings")]
    public float raycastDistance = 1000f;
    public LayerMask hitMask;

    [Header("Prefabs")]
    public GameObject modelPrefab;
    public GameObject impactPrefab;

    [Header("Positioning")]
    public Vector3 spawnPosition = Vector3.zero;
    public Vector3 spawnRotation = Vector3.zero;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyMagazineSound;
    public AudioClip impactSound;

    // ═══ NUEVO: SONIDO DE PICKUP DE MUNICIÓN ═══
    [Tooltip("Sonido al recoger munición (opcional)")]
    public AudioClip ammoPickupSound;

    [Header("Animations")]
    public AnimationClip idleAnimation;
    public AnimationClip recoilAnimation;
    public AnimationClip reloadAnimation;

    [Header("UI")]
    public Sprite weaponIcon;
    public Sprite bulletIcon;
}
