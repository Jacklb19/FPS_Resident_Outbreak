using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static event Action<int> OnScoreChanged;

    [Header("Scene Configuration")]
    private Dictionary<int, string> sceneByLevel;
    private string resultsScene = "04_GameOver";
    private string gameOverScene = "04_GameOver";
    private string loadingScene = "LoadingScreen";
    public static string targetScene = "";
    private static readonly Regex levelNameRegex = new Regex(@"^\d{2}_Level\d+$", RegexOptions.Compiled);

    [Header("Configuración")]
    public int masterVolume = 100;
    public int musicVolume = 100;
    public int sfxVolume = 100;
    public bool invertMouseY = false;
    public float mouseSensitivity = 1f;

    [Header("Progreso")]
    public int currentLevel = 1;
    public int totalScore = 0;
    public float playTime = 0f;

    [Header("Estado del Jugador")]
    public int playerHealth = 100;
    public int playerMaxHealth = 100;
    public string weapon1Name = "";
    public string weapon2Name = "";
    public int weapon1Ammo = 0;
    public int weapon2Ammo = 0;
    public int activeSlot = 0;
    public List<AmmoEntry> ammoEntries = new List<AmmoEntry>();

    private bool isPaused = false;

    [System.Serializable]
    public class AmmoEntry
    {
        public string weaponName;
        public int amount;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeScenes();
        LoadSettings();
        LoadProgress();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void InitializeScenes()
    {
        sceneByLevel = new Dictionary<int, string>
        {
            { 1, "01_Level1" },
            { 2, "02_Level2" },
            { 3, "03_Level3" }
        };
    }

    void Update()
    {
        if (!isPaused) playTime += Time.deltaTime;

    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SavePlayerState()
    {
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            playerHealth = player.CurrentHealth;
            playerMaxHealth = player.maxHealth;
            Debug.Log($"Salud guardada: {playerHealth}/{playerMaxHealth}");
        }

        var weaponInventory = FindObjectOfType<WeaponInventory>();
        if (weaponInventory != null)
        {
            var weapon1 = weaponInventory.GetWeaponInSlot(0);
            var weapon2 = weaponInventory.GetWeaponInSlot(1);

            weapon1Name = weapon1 != null && weapon1.weaponData != null ? weapon1.weaponData.weaponName : "";
            weapon2Name = weapon2 != null && weapon2.weaponData != null ? weapon2.weaponData.weaponName : "";
            weapon1Ammo = weapon1 != null ? weapon1.GetCurrentAmmo() : 0;
            weapon2Ammo = weapon2 != null ? weapon2.GetCurrentAmmo() : 0;
            activeSlot = weaponInventory.GetActiveSlotIndex();

            Debug.Log($"Armas guardadas: {weapon1Name} ({weapon1Ammo}), {weapon2Name} ({weapon2Ammo})");

            var ammoInventory = weaponInventory.GetAmmoInventory();
            ammoEntries.Clear();

            if (ammoInventory != null)
            {
                var allWeapons = Resources.LoadAll<WeaponData>("WeaponData");
                foreach (var weaponData in allWeapons)
                {
                    int ammo = ammoInventory.GetAmmo(weaponData);
                    if (ammo > 0)
                    {
                        ammoEntries.Add(new AmmoEntry { weaponName = weaponData.weaponName, amount = ammo });
                        Debug.Log($"Munición guardada: {weaponData.weaponName} = {ammo}");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No se encontró WeaponInventory para guardar!");
        }
    }


    public void RestorePlayerState()
    {
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.CurrentHealth = playerHealth;
            player.maxHealth = playerMaxHealth;
            Debug.Log($"Salud restaurada: {playerHealth}");
        }

        var weaponInventory = FindObjectOfType<WeaponInventory>();
        if (weaponInventory != null)
        {
            Debug.Log($"WeaponInventory encontrado. Restaurando armas...");

            if (!string.IsNullOrEmpty(weapon1Name))
            {
                Debug.Log($"Intentando cargar arma 1: WeaponData/{weapon1Name}");
                var weaponData1 = Resources.Load<WeaponData>($"WeaponData/{weapon1Name}");

                if (weaponData1 != null)
                {
                    Debug.Log($"WeaponData1 cargado: {weaponData1.weaponName}");

                    if (weaponData1.modelPrefab != null)
                    {
                        var instance1 = new WeaponInstance(weaponData1);
                        instance1.currentMagazineAmmo = weapon1Ammo;
                        weaponInventory.EquipWeapon(weaponData1.modelPrefab, instance1, 0);
                        Debug.Log($"Arma 1 equipada: {weapon1Name} con {weapon1Ammo} munición");
                    }
                    else
                    {
                        Debug.LogError($"modelPrefab es NULL en {weaponData1.weaponName}");
                    }
                }
                else
                {
                    Debug.LogError($"No se pudo cargar WeaponData: WeaponData/{weapon1Name}");
                }
            }

            if (!string.IsNullOrEmpty(weapon2Name))
            {
                Debug.Log($"Intentando cargar arma 2: WeaponData/{weapon2Name}");
                var weaponData2 = Resources.Load<WeaponData>($"WeaponData/{weapon2Name}");

                if (weaponData2 != null)
                {
                    Debug.Log($"WeaponData2 cargado: {weaponData2.weaponName}");

                    if (weaponData2.modelPrefab != null)
                    {
                        var instance2 = new WeaponInstance(weaponData2);
                        instance2.currentMagazineAmmo = weapon2Ammo;
                        weaponInventory.EquipWeapon(weaponData2.modelPrefab, instance2, 1);
                        Debug.Log($"Arma 2 equipada: {weapon2Name}");
                    }
                    else
                    {
                        Debug.LogError($"modelPrefab es NULL en {weaponData2.weaponName}");
                    }
                }
                else
                {
                    Debug.LogError($"No se pudo cargar WeaponData: WeaponData/{weapon2Name}");
                }
            }

            weaponInventory.SwitchToSlot(activeSlot);

            var ammoInventory = weaponInventory.GetAmmoInventory();
            if (ammoInventory != null)
            {
                foreach (var entry in ammoEntries)
                {
                    var weaponData = Resources.Load<WeaponData>($"WeaponData/{entry.weaponName}");
                    if (weaponData != null)
                    {
                        ammoInventory.SetAmmo(weaponData, entry.amount);
                        Debug.Log($"Munición restaurada: {entry.weaponName} = {entry.amount}");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("WeaponInventory NO encontrado!");
        }
    }


    public void LoadGameOver()
    {
        Time.timeScale = 1f;
        SaveProgress();
        LoadSceneWithLoadingScreen(gameOverScene);
    }

    public void LoadResults()
    {
        Time.timeScale = 1f;
        SaveProgress();
        LoadSceneWithLoadingScreen(resultsScene);
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        Time.timeScale = 1f;
        SaveProgress();

        if (sceneByLevel.ContainsKey(levelNumber))
        {
            LoadSceneWithLoadingScreen(sceneByLevel[levelNumber]);
        }
        else
        {
            LoadResults();
        }
    }

    public void LoadNextLevel()
    {
        SavePlayerState();

        var next = currentLevel + 1;

        if (sceneByLevel.ContainsKey(next))
        {
            LoadLevel(next);
        }
        else
        {
            LoadResults();
        }
    }

    private void LoadSceneWithLoadingScreen(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene(loadingScene);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;

        LoadSceneWithLoadingScreen(currentSceneName);
    }


    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SaveProgress();
        LoadSceneWithLoadingScreen("00_MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SaveProgress();
        Application.Quit();
    }

    public void AddScore(int points)
    {
        totalScore += points;
        OnScoreChanged?.Invoke(totalScore);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("MasterVolume", masterVolume);
        PlayerPrefs.SetInt("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("InvertMouseY", invertMouseY ? 1 : 0);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetInt("MasterVolume", 100);
        musicVolume = PlayerPrefs.GetInt("MusicVolume", 100);
        sfxVolume = PlayerPrefs.GetInt("SFXVolume", 100);
        invertMouseY = PlayerPrefs.GetInt("InvertMouseY", 0) == 1;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.SetFloat("PlayTime", playTime);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        playTime = PlayerPrefs.GetFloat("PlayTime", 0f);
    }

    public void ResetProgress()
    {
        currentLevel = 1;
        totalScore = 0;
        playTime = 0f;
        playerHealth = 100;
        playerMaxHealth = 100;
        weapon1Name = "";
        weapon2Name = "";
        weapon1Ammo = 0;
        weapon2Ammo = 0;
        activeSlot = 0;
        ammoEntries.Clear();
        SaveProgress();
    }

    public bool IsPaused() => isPaused;

    private bool IsLevelScene(string sceneName)
    {
        return levelNameRegex.IsMatch(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsLevelScene(scene.name))
        {
            UpdateCurrentLevelFromScene(scene.name);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var sm = FindObjectOfType<SpawnManager>();
            if (sm != null)
            {
                sm.Begin();
            }

            StartCoroutine(RestorePlayerStateDelayed());
        }

    }
    private void UpdateCurrentLevelFromScene(string sceneName)
    {

        foreach (var kvp in sceneByLevel)
        {
            if (kvp.Value == sceneName)
            {
                currentLevel = kvp.Key;
                Debug.Log($"Current level actualizado a: {currentLevel}");
                return;
            }
        }
    }
    private IEnumerator RestorePlayerStateDelayed()
    {
        // Esperar más tiempo para que todo se inicialice
        yield return new WaitForSeconds(0.2f);

        Debug.Log($"Intentando restaurar estado. Nivel actual: {currentLevel}");
        Debug.Log($"Armas guardadas: weapon1={weapon1Name}, weapon2={weapon2Name}");

        // Restaurar siempre si hay armas guardadas O si el nivel es mayor a 1
        if (currentLevel > 1 || !string.IsNullOrEmpty(weapon1Name) || !string.IsNullOrEmpty(weapon2Name))
        {
            Debug.Log("Restaurando estado del jugador...");
            RestorePlayerState();
        }
        else
        {
            Debug.Log("No hay estado para restaurar (primer nivel o sin armas guardadas)");
        }
    }


}
