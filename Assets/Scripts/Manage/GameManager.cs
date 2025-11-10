using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static event Action<int> OnScoreChanged;

    [Header("Scene Configuration")]
    private Dictionary<int, string> sceneByLevel;
    private string resultsScene = "Scenes/04_GameOver";

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

        // Suscribir a evento de carga de escenas
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Garantiza arranque si presionas Play directo en nivel
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void InitializeScenes()
    {
        sceneByLevel = new Dictionary<int, string>
        {
            { 1, "Scenes/01_Level1" },
            { 2, "Scenes/02_Level2" },
            { 3, "Scenes/03_Level3" }
        };
    }

    void Update()
    {
        if (!isPaused)
        {
            playTime += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "00_MainMenu")
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Juego pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Juego reanudado");
    }

    public void LoadNextLevel()
    {
        var next = currentLevel + 1;
        if (sceneByLevel.ContainsKey(next))
        {
            LoadLevel(next);
        }
        else
        {
            SceneManager.LoadScene(resultsScene);
        }
    }

    public void LoadResults()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(resultsScene);
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        Time.timeScale = 1f;

        if (sceneByLevel.ContainsKey(levelNumber))
        {
            SceneManager.LoadScene(sceneByLevel[levelNumber]);
        }
        else
        {
            SceneManager.LoadScene(resultsScene);
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SceneManager.LoadScene("Scenes/00_MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SaveSettings();
        Debug.Log("Saliendo del juego");
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

    public bool IsPaused()
    {
        return isPaused;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] sceneLoaded: {scene.name}");

        // Quita "Scenes/" del nombre para coincidir con scene.name
        if (scene.name == "01_Level1" || scene.name == "02_Level2" || scene.name == "03_Level3")
        {
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
