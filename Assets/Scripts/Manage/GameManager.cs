using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static event Action<int> OnScoreChanged;

    [Header("Scene Configuration")]
    private Dictionary<int, string> sceneByLevel;
    private string resultsScene = "04_GameOver";
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

    private bool isPaused = false;

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

        if (Input.GetKeyDown(KeyCode.Escape) && !SceneManager.GetActiveScene().name.Contains("MainMenu"))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("[GameManager] Juego pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("[GameManager] Juego reanudado");
    }


    public void LoadResults()
    {
        Time.timeScale = 1f;
        SaveProgress();
        SceneManager.LoadScene(resultsScene);
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        Time.timeScale = 1f;

        // ✅ Guardar progreso ANTES de cargar la escena
        SaveProgress();

        if (sceneByLevel.ContainsKey(levelNumber))
        {
            Debug.Log($"[GameManager] Cargando nivel {levelNumber}: {sceneByLevel[levelNumber]}");
            SceneManager.LoadScene(sceneByLevel[levelNumber]);
        }
        else
        {
            Debug.LogWarning($"[GameManager] Nivel {levelNumber} no existe, cargando pantalla de resultados");
            LoadResults();
        }
    }

    public void LoadNextLevel()
    {
        var next = currentLevel + 1;
        Debug.Log($"[GameManager] LoadNextLevel: currentLevel={currentLevel} → next={next}");

        if (sceneByLevel.ContainsKey(next))
        {
            LoadLevel(next);
        }
        else
        {
            Debug.Log($"[GameManager] No hay más niveles, cargando pantalla de resultados");
            LoadResults();
        }
    }


    public void RestartLevel()
    {
        Time.timeScale = 1f;
        if (sceneByLevel.TryGetValue(currentLevel, out var sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            LoadResults();
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SaveProgress();
        SceneManager.LoadScene("00_MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SaveProgress();
        Debug.Log("[GameManager] Saliendo del juego");
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
        SaveProgress();
    }

    public bool IsPaused() => isPaused;

    private bool IsLevelScene(string sceneName)
    {
        return levelNameRegex.IsMatch(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] sceneLoaded: {scene.name}");

        if (IsLevelScene(scene.name))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var sm = FindObjectOfType<SpawnManager>();
            if (sm != null)
            {
                Debug.Log("[GameManager] SpawnManager encontrado, llamando Begin()");
                sm.Begin();
            }
            else
            {
                Debug.LogWarning("[GameManager] SpawnManager no encontrado en escena.");
            }
        }
    }
}
