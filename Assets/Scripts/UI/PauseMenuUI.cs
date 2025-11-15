using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine; // ← Añade esto

public class PauseMenuUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pauseMenuPanel;
    public GameObject hudRoot;              // ← Padre del HUD (Health, Puntuación, etc.)

    public Button btnResume;
    public Button btnRestart;
    public Button btnMainMenu;
    public Button btnQuit;

    [Header("Control del Jugador")]
    public CinemachineVirtualCamera virtualCamera;
    public MonoBehaviour playerMovementScript;

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (btnResume != null) btnResume.onClick.AddListener(Resume);
        if (btnRestart != null) btnRestart.onClick.AddListener(Restart);
        if (btnMainMenu != null) btnMainMenu.onClick.AddListener(BackToMainMenu);
        if (btnQuit != null) btnQuit.onClick.AddListener(QuitGame);
    }

    public void Pause()
    {
        Debug.Log("Pausando - HUD Root: " + (hudRoot != null ? hudRoot.name : "NULL"));
        Debug.Log("Pausando - PauseMenu: " + (pauseMenuPanel != null ? pauseMenuPanel.name : "NULL"));

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (hudRoot != null)
            hudRoot.SetActive(false);

        Debug.Log("HUD desactivado, PauseMenu debería estar visible");

        if (virtualCamera != null)
            virtualCamera.enabled = false;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        if (GameManager.instance != null)
            GameManager.instance.PauseGame();

        var gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
            gameUI.SetCrosshairVisible(false);
    }

    public void Resume()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (hudRoot != null)
            hudRoot.SetActive(true);    // Muestra el HUD

        if (virtualCamera != null)
            virtualCamera.enabled = true;

        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        if (GameManager.instance != null)
            GameManager.instance.ResumeGame();

        var gameUI = FindObjectOfType<GameUI>();
        if (gameUI != null)
            gameUI.SetCrosshairVisible(true);
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

    void Update()
{
    // Solo escucha ESC si no está el menú de configuración/conversación abierto
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (pauseMenuPanel != null && !pauseMenuPanel.activeSelf)
            Pause();     // Abre menú de pausa
        else if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
            Resume();    // Cierra menú de pausa
    }
}

}