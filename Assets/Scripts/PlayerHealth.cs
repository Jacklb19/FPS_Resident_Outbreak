using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Efectos")]
    public Image damageOverlay;
    public float damageFadeSpeed = 2f;
    
    private float targetAlpha = 0f;
    
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Start()
    {
        currentHealth = maxHealth;
        
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
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        Debug.Log("Jugador recibió " + damage + " de daño. Vida actual: " + currentHealth);
        
        ShowDamageEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log("Jugador curado. Vida actual: " + currentHealth);
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
        Debug.Log("Jugador ha muerto");
        GetComponent<CharacterController>().enabled = false;
        Time.timeScale = 0f;
    }
}
