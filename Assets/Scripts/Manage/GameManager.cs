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
    private string resultsScene = "05_Victory";
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

    [Header("Score Breakdown")]
    public int zombiesKilled = 0;
    public int generatorsActivated = 0;
    public int bossKilled = 0;
    public int medicalPackagesCollected = 0;
    public int wavesCompleted = 0;

    public int scorePerZombie = 100;
    public int scorePerGenerator = 200;
    public int scorePerBoss = 5000;
    public int scorePerPackage = 50;
    public int scorePerWave = 500;
    [Header("Resumen por Mundo")]
    public List<int> worldScores = new List<int>();
    private int totalWorlds = 3;
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
    private bool isVictory = false;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void SaveWorldScore()
    {
        // Expande la lista si no es suficiente
        while (worldScores.Count < currentLevel)
            worldScores.Add(0);

        worldScores[currentLevel - 1] = totalScore;
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
        if (!isPaused && !isVictory) playTime += Time.deltaTime;
    }

    public void PauseGame()
    {
        if (isVictory) return;

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

            var ammoInventory = weaponInventory.GetAmmoInventory();
            ammoEntries.Clear();

            if (ammoInventory != null)
            {
                var allWeapons = Resources.LoadAll<WeaponData>("WeaponData");
                foreach (var weaponData in allWeapons)
                {
                    int ammo = ammoInventory.GetAmmo(weaponData);
                    if (ammo > 0)
                        ammoEntries.Add(new AmmoEntry { weaponName = weaponData.weaponName, amount = ammo });
                }
            }
        }
    }

    public void RestorePlayerState()
    {
        var player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.CurrentHealth = playerHealth;
            player.maxHealth = playerMaxHealth;
        }

        var weaponInventory = FindObjectOfType<WeaponInventory>();
        if (weaponInventory != null)
        {
            if (!string.IsNullOrEmpty(weapon1Name))
            {
                var weaponData1 = Resources.Load<WeaponData>($"WeaponData/{weapon1Name}");
                if (weaponData1 != null && weaponData1.modelPrefab != null)
                {
                    var instance1 = new WeaponInstance(weaponData1);
                    instance1.currentMagazineAmmo = weapon1Ammo;
                    weaponInventory.EquipWeapon(weaponData1.modelPrefab, instance1, 0);
                }
            }

            if (!string.IsNullOrEmpty(weapon2Name))
            {
                var weaponData2 = Resources.Load<WeaponData>($"WeaponData/{weapon2Name}");
                if (weaponData2 != null && weaponData2.modelPrefab != null)
                {
                    var instance2 = new WeaponInstance(weaponData2);
                    instance2.currentMagazineAmmo = weapon2Ammo;
                    weaponInventory.EquipWeapon(weaponData2.modelPrefab, instance2, 1);
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
                        ammoInventory.SetAmmo(weaponData, entry.amount);
                }
            }
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
        isVictory = true;
        Time.timeScale = 0f;
        SaveWorldScore();
        SaveProgress();
        SceneManager.LoadScene(resultsScene);
    }

    public void AddScore(int points)
    {
        totalScore += points;
        OnScoreChanged?.Invoke(totalScore);
    }

    public void AddZombieKill() { zombiesKilled++; AddScore(scorePerZombie); }
    public void AddGeneratorActivated() { generatorsActivated++; AddScore(scorePerGenerator); }
    public void AddBossKill() { bossKilled++; AddScore(scorePerBoss); }
    public void AddMedicalPackage() { medicalPackagesCollected++; AddScore(scorePerPackage); }
    public void AddWaveCompleted() { wavesCompleted++; AddScore(scorePerWave); }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber == 1)
        {
            playTime = 0f;
            totalScore = 0;
            zombiesKilled = 0;
            generatorsActivated = 0;
            bossKilled = 0;
            medicalPackagesCollected = 0;
            wavesCompleted = 0;

            worldScores = new List<int>();
        }
        currentLevel = levelNumber;
        Time.timeScale = 1f;
        isVictory = false;
        SaveProgress();

        if (sceneByLevel.ContainsKey(levelNumber))
            LoadSceneWithLoadingScreen(sceneByLevel[levelNumber]);
        else
            LoadResults();
    }


    public void LoadNextLevel()
    {
        SaveWorldScore();
        SavePlayerState();
        var next = currentLevel + 1;
        if (sceneByLevel.ContainsKey(next))
            LoadLevel(next);
        else
            LoadResults();
    }
    private void LoadSceneWithLoadingScreen(string sceneName)
    {
        targetScene = sceneName;
        SceneManager.LoadScene(loadingScene);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isVictory = false;
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadSceneWithLoadingScreen(currentSceneName);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isVictory = false;
        SaveSettings();
        SaveProgress();
        SceneManager.LoadScene("00_MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SaveProgress();
        Application.Quit();
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
        zombiesKilled = 0;
        generatorsActivated = 0;
        bossKilled = 0;
        medicalPackagesCollected = 0;
        wavesCompleted = 0;
        SaveProgress();
    }

    public bool IsPaused() => isPaused;
    public bool IsVictory() => isVictory;

    private bool IsLevelScene(string sceneName) => levelNameRegex.IsMatch(sceneName);

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (IsLevelScene(scene.name))
        {
            isVictory = false;
            UpdateCurrentLevelFromScene(scene.name);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var sm = FindObjectOfType<SpawnManager>();
            if (sm != null)
                sm.Begin();

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
                return;
            }
        }
    }

    private IEnumerator RestorePlayerStateDelayed()
    {
        yield return new WaitForSeconds(0.2f);
        if (currentLevel > 1 || !string.IsNullOrEmpty(weapon1Name) || !string.IsNullOrEmpty(weapon2Name))
            RestorePlayerState();
    }
}
