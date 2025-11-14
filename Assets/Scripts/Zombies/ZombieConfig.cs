using UnityEngine;

[CreateAssetMenu(fileName = "ZombieConfig", menuName = "Zombies/Zombie Config")]
public class ZombieConfig : ScriptableObject
{
    [Header("Rangos")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float loseInterestDistance = 20f;

    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public int rangedDamage = 15;
    public int areaDamage = 5;
    public float backstabMultiplier = 2f;

    [Header("Ataque a distancia")]
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float aimHeightOffset = 1.2f;
    public float rangedAttackRange = 15f;

    [Header("Movimiento")]
    public float pursuitSpeed = 1.5f;

    [Header("Vida")]
    public int maxHealth = 100;

    [Header("Audio")]
    public AudioClip[] idleGroans;
    public AudioClip[] hitSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] attackSounds;
    public float idleGroanIntervalMin = 2f;
    public float idleGroanIntervalMax = 5f;
}