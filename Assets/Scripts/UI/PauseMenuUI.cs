using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine; // ← Añade esto

public class PauseMenuUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pauseMenuPanel;
    public Canvas pauseCanvas;
    public Button btnResume;
    public Button btnRestart;
    public Button btnMainMenu;
    public Button btnQuit;
    
    [Header("Control del Jugador")]
    public CinemachineVirtualCamera virtualCamera; // ← Cambia a esto
    public MonoBehaviour playerMovementScript;

    void Start()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.sortingOrder = 100;
        }

        if (btnResume != null)
            btnResume.onClick.AddListener(Resume);

        if (btnRestart != null)
            btnRestart.onClick.AddListener(Restart);

        if (btnMainMenu != null)
            btnMainMenu.onClick.AddListener(BackToMainMenu);

        if (btnQuit != null)
            btnQuit.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.instance != null)
            {
                if (GameManager.instance.IsPaused())
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    public void Pause()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Desactivar Cinemachine Virtual Camera
        if (virtualCamera != null)
        {
            virtualCamera.enabled = false;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.PauseGame();
        }

        var gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            gameUI.SetCrosshairVisible(false);
        }
    }

    public void Resume()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Reactivar Cinemachine Virtual Camera
        if (virtualCamera != null)
        {
            virtualCamera.enabled = true;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ResumeGame();
        }

        var gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
        {
            gameUI.SetCrosshairVisible(true);
        }
    }

    public void Restart()
    {
        if (virtualCamera != null)
            virtualCamera.enabled = true;
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.RestartLevel();
        }
    }

    public void BackToMainMenu()
    {
        if (virtualCamera != null)
            virtualCamera.enabled = true;
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.LoadMainMenu();
        }
    }

    public void QuitGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.QuitGame();
        }
    }
}
