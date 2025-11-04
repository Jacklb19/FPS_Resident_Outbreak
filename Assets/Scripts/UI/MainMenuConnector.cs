using UnityEngine;
using UnityEngine.UI;

public class MainMenuConnector : MonoBehaviour
{
    [Header("Botones del Asset")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;

    void Start()
    {
        // Conectar botones a GameManager
        if (playButton != null)
            playButton.onClick.AddListener(() => GameManager.instance.LoadLevel(1));
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(() => GameManager.instance.QuitGame());
    }

    void OnSettingsClicked()
    {
        // El asset ya tiene un panel de configuración
        // Solo necesitas mostrar/ocultar
        Debug.Log("Abrir configuración");
    }

    void OnCreditsClicked()
    {
        Debug.Log("Mostrar créditos");
    }
}
