using UnityEngine;

[CreateAssetMenu(fileName = "ZombieConfig", menuName = "Zombies/Zombie Config")]
public class ZombieConfig : ScriptableObject
{
    [Header("Rangos")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float loseInterestDistance = 20f; // opcional

    [Header("Ataque")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public bool isRanged = false;           // Spitter
    public int rangedDamage = 15;           // impacto directo
    public int areaDamage = 5;              // charco o salpicadura
    public float backstabMultiplier = 2f;   // Crawler

    [Header("Movimiento")]
    public float pursuitSpeed = 1.5f;

    [Header("Vida")]
    public int maxHealth = 100;

    [Header("Audio")]
    public AudioClip[] idleGroans; // Sonidos de gruñido/gruñido ambiente
    public AudioClip[] hitSounds;  // Sonido daño
    public AudioClip[] deathSounds;// Sonido muerte
    public AudioClip[] attackSounds;
    public AudioClip spawnSound;
    public float idleGroanIntervalMin = 2f;
    public float idleGroanIntervalMax = 5f;
}
