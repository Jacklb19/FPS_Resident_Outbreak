using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerHealth playerHealth;

    [Header("UI - WeaponPanel del Asset")]
    public TextMeshProUGUI magazineAmmoText;
    public TextMeshProUGUI totalAmmoText;
    public Image ammoTypeIcon;
    public Image activeWeaponIcon;
    public Image unActiveWeaponIcon;
    public Image lethalIcon;
    public Image tacticalIcon;

    [Header("UI - Salud")]
    public Slider playerHealthBar;
    public TextMeshProUGUI healthText;

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

    void UpdateAmmoDisplay()
    {
        Weapon currentWeapon = GetActiveWeapon();

        if (currentWeapon != null)
        {
            int currentAmmo = currentWeapon.GetCurrentAmmo();
            int totalAmmo = currentWeapon.GetTotalAmmo();

            if (magazineAmmoText != null)
            {
                magazineAmmoText.text = currentAmmo.ToString();
                magazineAmmoText.gameObject.SetActive(true);
            }

            if (totalAmmoText != null)
            {
                totalAmmoText.text = totalAmmo.ToString();
                totalAmmoText.gameObject.SetActive(true);
            }

            if (ammoTypeIcon != null && currentWeapon.weaponData != null)
            {
                if (currentWeapon.weaponData.bulletIcon != null)
                {
                    ammoTypeIcon.sprite = currentWeapon.weaponData.bulletIcon;
                    ammoTypeIcon.gameObject.SetActive(true);
                }
                else
                {
                    ammoTypeIcon.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (magazineAmmoText != null)
                magazineAmmoText.gameObject.SetActive(false);

            if (totalAmmoText != null)
                totalAmmoText.gameObject.SetActive(false);

            if (ammoTypeIcon != null)
                ammoTypeIcon.gameObject.SetActive(false);
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
        Weapon currentWeapon = GetActiveWeapon();

        if (currentWeapon != null)
        {
            if (activeWeaponIcon != null)
            {
                activeWeaponIcon.gameObject.SetActive(true);
                if (currentWeapon.weaponData != null && currentWeapon.weaponData.weaponIcon != null)
                {
                    activeWeaponIcon.sprite = currentWeapon.weaponData.weaponIcon;
                }
            }

            Weapon secondaryWeapon = GetInactiveWeapon();

            if (unActiveWeaponIcon != null)
            {
                if (secondaryWeapon != null && secondaryWeapon.weaponData != null)
                {
                    if (secondaryWeapon.weaponData.weaponIcon != null)
                    {
                        unActiveWeaponIcon.sprite = secondaryWeapon.weaponData.weaponIcon;
                        unActiveWeaponIcon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    unActiveWeaponIcon.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (activeWeaponIcon != null)
                activeWeaponIcon.gameObject.SetActive(false);

            if (unActiveWeaponIcon != null)
                unActiveWeaponIcon.gameObject.SetActive(false);
        }
    }

    private Weapon GetActiveWeapon()
    {
        if (WeaponManager.instance == null)
            return null;

        Transform activeSlot = WeaponManager.instance.GetActiveSlot();
        
        if (activeSlot != null && activeSlot.childCount > 0)
        {
            return activeSlot.GetChild(0).GetComponent<Weapon>();
        }

        return null;
    }

    private Weapon GetInactiveWeapon()
    {
        if (WeaponManager.instance == null)
            return null;

        Transform activeSlot = WeaponManager.instance.GetActiveSlot();
        Transform[] allSlots = WeaponManager.instance.GetAllSlots();

        if (allSlots != null)
        {
            foreach (Transform slot in allSlots)
            {
                if (slot != activeSlot && slot.childCount > 0)
                {
                    return slot.GetChild(0).GetComponent<Weapon>();
                }
            }
        }

        return null;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + currentScore;
        }
    }

    private void CreateCrosshair()
    {
        GameObject crosshairObj = new GameObject("CrosshairImage");
        crosshairObj.transform.SetParent(transform);
        crosshairObj.transform.localPosition = Vector3.zero;

        crosshairImage = crosshairObj.AddComponent<Image>();

        if (crosshairSprite != null)
        {
            crosshairImage.sprite = crosshairSprite;
        }
        else
        {
            Debug.LogWarning("Crosshair Sprite no asignado en GameUI");
        }

        crosshairImage.preserveAspect = true;

        crosshairRect = crosshairObj.GetComponent<RectTransform>();
        crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
        crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
        crosshairRect.pivot = new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition = Vector2.zero;
        crosshairRect.sizeDelta = new Vector2(normalSize, normalSize);

        Debug.Log("Mira creada en GameUI");
    }

    private void UpdateCrosshair()
    {
        if (crosshairImage == null)
            return;

        Weapon currentWeapon = GetActiveWeapon();

        if (currentWeapon != null)
        {
            crosshairImage.gameObject.SetActive(true);
            UpdateCrosshairColor();
            UpdateCrosshairSize();
        }
        else
        {
            crosshairImage.gameObject.SetActive(false);
        }
    }

    private void UpdateCrosshairColor()
    {
        Color displayColor = normalColor;

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
