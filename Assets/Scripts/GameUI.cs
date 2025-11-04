using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerHealth playerHealth;
    public WeaponController weaponController;

    [Header("UI - Ammo")]
    public TextMeshProUGUI ammoText;

    [Header("UI - Salud")]
    public Slider playerHealthBar;
    public TextMeshProUGUI healthText;

    [Header("UI - Arma")]
    public TextMeshProUGUI weaponNameText;
    public Image weaponIcon;

    [Header("UI - Puntuación")]
    public TextMeshProUGUI scoreText;

    private int currentScore = 0;

    void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: 0";
        }
    }


    void Update()
    {
        UpdateAmmoDisplay();
        UpdateHealthDisplay();
        UpdateWeaponDisplay();
    }


    void UpdateAmmoDisplay()
    {
        if (weaponController != null && ammoText != null)
        {
            ammoText.text = weaponController.currentAmmo + " / " + weaponController.reserveAmmo;
        }
    }


    void UpdateHealthDisplay()
    {
        if (playerHealth != null && playerHealthBar != null && healthText != null)
        {
            float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.maxHealth;
            playerHealthBar.value = healthPercent;
            healthText.text = playerHealth.CurrentHealth + " / " + playerHealth.maxHealth;
        }
    }


    void UpdateWeaponDisplay()
    {
        if (weaponController != null && weaponNameText != null)
        {
            weaponNameText.text = weaponController.weaponName;

            if (weaponIcon != null)
            {
                weaponIcon.sprite = weaponController.weaponIcon;
            }
        }
    }


    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore;
        }
    }
}
