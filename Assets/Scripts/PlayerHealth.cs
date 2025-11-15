using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Efectos")]
    public Image damageOverlay;
    public float damageFadeSpeed = 2f;

    [Header("Sonidos")]
    public AudioClip damageSound;      // Clip de daño
    public AudioSource audioSource;

    private float targetAlpha = 0f;

    // Evento para avisar cambios en la salud a la UI u otros sistemas
    public event Action<int> OnHealthChanged;

    public int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            int newValue = Mathf.Clamp(value, 0, maxHealth);
            if (currentHealth != newValue)
            {
                currentHealth = newValue;
                OnHealthChanged?.Invoke(currentHealth);
            }
        }
    }

    void Start()
    {
        CurrentHealth = maxHealth;

        if (damageOverlay != null)
        {
            Color color = damageOverlay.color;
            color.a = 0f;
            damageOverlay.color = color;
        }
    }

    void Update()
    {
        if (damageOverlay != null)
        {
            Color color = damageOverlay.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, damageFadeSpeed * Time.deltaTime);
            damageOverlay.color = color;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log($"TakeDamage: vida actual = {CurrentHealth} (en {gameObject.name})");

        ShowDamageEffect();

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
    }

    void ShowDamageEffect()
    {
        if (damageOverlay != null)
        {
            targetAlpha = 0.3f;
            StartCoroutine(FadeDamageEffect());
        }
    }

    System.Collections.IEnumerator FadeDamageEffect()
    {
        yield return new WaitForSeconds(0.1f);
        targetAlpha = 0f;
    }

    void Die()
    {
        GetComponent<CharacterController>().enabled = false;

        if (GameManager.instance != null)
        {
            GameManager.instance.LoadGameOver();
        }
    }
}
