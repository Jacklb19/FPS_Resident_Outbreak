using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerHealth playerHealth;

    [Header("UI - Ammo")]
    public TextMeshProUGUI ammoText;

    [Header("UI - Salud")]
    public Slider playerHealthBar;
    public TextMeshProUGUI healthText;

    [Header("UI - Arma")]
    public TextMeshProUGUI weaponNameText;
    public Image weaponIcon;
    public Image secondaryWeaponIcon;

    [Header("UI - Puntuación")]
    public TextMeshProUGUI scoreText;

    [Header("UI - Mira")]
    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitDuration = 0.1f;
    [SerializeField] private float normalSize = 128f;
    [SerializeField] private float expandedSize = 160f;
    [SerializeField] private float expandDuration = 0.15f;

    private int currentScore = 0;
    private Image crosshairImage;
    private RectTransform crosshairRect;
    private float lastHitTime;
    private float lastShotTime;

    void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: 0";
        }

        CreateCrosshair();
    }

    void Update()
    {
        UpdateAmmoDisplay();
        UpdateHealthDisplay();
        UpdateWeaponDisplay();
        UpdateCrosshair();
    }

    // ===== MÉTODOS DE MUNICIÓN =====

    void UpdateAmmoDisplay()
    {
        PlayerWeaponController playerWeaponController = FindObjectOfType<PlayerWeaponController>();

        if (playerWeaponController != null && ammoText != null)
        {
            Weapon currentWeapon = playerWeaponController.GetActiveWeapon();

            if (currentWeapon != null)
            {
                int currentAmmo = currentWeapon.GetCurrentAmmo();
                int totalAmmo = currentWeapon.GetTotalAmmo();
                ammoText.text = currentAmmo + " / " + totalAmmo;
            }
            else
            {
                ammoText.text = "--- / ---";
            }
        }
    }


    // ===== MÉTODOS DE SALUD =====

    void UpdateHealthDisplay()
    {
        if (playerHealth != null && playerHealthBar != null && healthText != null)
        {
            float healthPercent = (float)playerHealth.CurrentHealth / playerHealth.maxHealth;
            playerHealthBar.value = healthPercent;
            healthText.text = playerHealth.CurrentHealth + " / " + playerHealth.maxHealth;
        }
    }

    // ===== MÉTODOS DE ARMA =====

void UpdateWeaponDisplay()
{
    PlayerWeaponController playerWeaponController = FindObjectOfType<PlayerWeaponController>();
    
    if (playerWeaponController != null)
    {
        // Arma ACTIVA
        Weapon currentWeapon = playerWeaponController.GetActiveWeapon();
        
        if (currentWeapon != null) // ✅ SI TIENE ARMA
        {
            // Mostrar todo
            if (weaponNameText != null)
            {
                weaponNameText.gameObject.SetActive(true);
                weaponNameText.text = currentWeapon.GetWeaponName();
            }

            if (weaponIcon != null)
            {
                weaponIcon.gameObject.SetActive(true);
                if (currentWeapon.weaponData != null && currentWeapon.weaponData.weaponIcon != null)
                {
                    weaponIcon.sprite = currentWeapon.weaponData.weaponIcon;
                }
            }

            if (ammoText != null)
            {
                ammoText.gameObject.SetActive(true);
                int currentAmmo = currentWeapon.GetCurrentAmmo();
                int totalAmmo = currentWeapon.GetTotalAmmo();
                ammoText.text = currentAmmo + " / " + totalAmmo;
            }

            // Arma GUARDADA
            int activeSlot = playerWeaponController.GetActiveSlotIndex();
            int inactiveSlot = 1 - activeSlot;
            Weapon secondaryWeapon = playerWeaponController.GetWeaponInSlot(inactiveSlot);

            if (secondaryWeaponIcon != null)
            {
                if (secondaryWeapon != null && secondaryWeapon.weaponData != null)
                {
                    if (secondaryWeapon.weaponData.weaponIcon != null)
                    {
                        secondaryWeaponIcon.sprite = secondaryWeapon.weaponData.weaponIcon;
                        secondaryWeaponIcon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    secondaryWeaponIcon.gameObject.SetActive(false);
                }
            }
        }
        else // ✅ SIN ARMA
        {
            // Ocultar todo
            if (weaponNameText != null)
                weaponNameText.gameObject.SetActive(false);

            if (weaponIcon != null)
                weaponIcon.gameObject.SetActive(false);

            if (ammoText != null)
                ammoText.gameObject.SetActive(false);

            if (secondaryWeaponIcon != null)
                secondaryWeaponIcon.gameObject.SetActive(false);
        }
    }
}



    // ===== PUNTUACIÓN =====

    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore;
        }
    }

    // ===== MÉTODOS DE MIRA =====

    private void CreateCrosshair()
    {
        // Crear GameObject para la mira
        GameObject crosshairObj = new GameObject("CrosshairImage");
        crosshairObj.transform.SetParent(transform); // Hijo del Canvas
        crosshairObj.transform.localPosition = Vector3.zero;

        // Agregar Image
        crosshairImage = crosshairObj.AddComponent<Image>();

        // Asignar sprite
        if (crosshairSprite != null)
        {
            crosshairImage.sprite = crosshairSprite;
        }
        else
        {
            Debug.LogWarning("Crosshair Sprite no asignado en GameUI");
        }

        crosshairImage.preserveAspect = true;

        // Configurar RectTransform
        crosshairRect = crosshairObj.GetComponent<RectTransform>();
        crosshairRect.anchorMin = new Vector2(0.5f, 0.5f); // Centro
        crosshairRect.anchorMax = new Vector2(0.5f, 0.5f); // Centro
        crosshairRect.pivot = new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition = Vector2.zero;
        crosshairRect.sizeDelta = new Vector2(normalSize, normalSize);

        Debug.Log("Mira creada en GameUI");
    }

private void UpdateCrosshair()
{
    if (crosshairImage == null)
        return;

    // ✅ Verificar si tiene arma equipada
    PlayerWeaponController playerWeaponController = FindObjectOfType<PlayerWeaponController>();
    Weapon currentWeapon = playerWeaponController != null ? playerWeaponController.GetActiveWeapon() : null;

    if (currentWeapon != null)
    {
        // ✅ Mostrar mira si tiene arma
        crosshairImage.gameObject.SetActive(true);
        UpdateCrosshairColor();
        UpdateCrosshairSize();
    }
    else
    {
        // ✅ Ocultar mira si NO tiene arma
        crosshairImage.gameObject.SetActive(false);
    }
}

    private void UpdateCrosshairColor()
    {
        Color displayColor = normalColor;

        // Si fue hit recientemente, cambiar a rojo
        if (Time.time - lastHitTime < hitDuration)
        {
            float t = (Time.time - lastHitTime) / hitDuration;
            displayColor = Color.Lerp(hitColor, normalColor, t);
        }

        crosshairImage.color = displayColor;
    }

    private void UpdateCrosshairSize()
    {
        float currentSize = normalSize;

        // Si disparó recientemente, expandir
        if (Time.time - lastShotTime < expandDuration)
        {
            float t = (Time.time - lastShotTime) / expandDuration;
            currentSize = Mathf.Lerp(expandedSize, normalSize, t);
        }

        crosshairRect.sizeDelta = new Vector2(currentSize, currentSize);
    }

    public void OnCrosshairShot()
    {
        lastShotTime = Time.time;
    }

    public void OnCrosshairHit()
    {
        lastHitTime = Time.time;
    }
}
