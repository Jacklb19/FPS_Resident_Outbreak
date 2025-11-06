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
    public float spread = 0.1f;
    public int damage = 25;
    
    [Header("Magazine Settings")]
    public int magazineSize = 30;
    public float reloadTime = 2f;
    
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

    [Header("Animations")]
    public AnimationClip idleAnimation;
    public AnimationClip recoilAnimation;
    public AnimationClip reloadAnimation;

    [Header("UI")]
    public Sprite weaponIcon;
}
