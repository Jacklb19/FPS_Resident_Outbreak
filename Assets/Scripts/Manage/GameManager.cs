using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

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
        LoadSettings();
    }

    void Update()
    {
        if (!isPaused)
        {
            playTime += Time.deltaTime;
        }

        // Pausar con ESC
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "00_MainMenu")
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        // Aquí integrarás con el sistema de pausa del asset
        Debug.Log("Juego pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Juego reanudado");
    }

    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        Time.timeScale = 1f;
        SceneManager.LoadScene($"0{levelNumber}_Level{levelNumber}");
        Debug.Log($"Cargando nivel {levelNumber}");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SaveSettings();
        SceneManager.LoadScene("00_MainMenu");
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
}
