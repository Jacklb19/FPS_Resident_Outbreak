using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuConnector : MonoBehaviour
{
    [Header("Main Buttons")]
    public Button btnNewGame;
    public Button btnContinue;
    public Button btnLoadGame;
    public Button btnExit_Yes;
    public Button btnExit_No;

    void Start()
    {
        // PLAY
        if (btnNewGame != null)
            btnNewGame.onClick.AddListener(() => {
                GameManager.instance.currentLevel = 1;
                GameManager.instance.totalScore = 0;
                GameManager.instance.playTime = 0f;
                GameManager.instance.LoadLevel(1);
            });

        if (btnContinue != null)
            btnContinue.onClick.AddListener(() => 
                GameManager.instance.LoadLevel(GameManager.instance.currentLevel)
            );

        if (btnLoadGame != null)
            btnLoadGame.onClick.AddListener(() => 
                GameManager.instance.LoadLevel(1) // O cargar última guardada
            );

        // EXIT
        if (btnExit_Yes != null)
            btnExit_Yes.onClick.AddListener(() => 
                GameManager.instance.QuitGame()
            );

        if (btnExit_No != null)
            btnExit_No.onClick.AddListener(() => 
                Debug.Log("Cancelar salida") // El asset ya lo maneja visualmente
            );
    }
}
