using UnityEngine;
using UnityEngine.UI;

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
        if (btnNewGame != null)
            btnNewGame.onClick.AddListener(() => {
                GameManager.instance.ResetProgress();
                GameManager.instance.LoadLevel(1);
            });

        if (btnContinue != null)
            btnContinue.onClick.AddListener(() => 
                GameManager.instance.LoadLevel(GameManager.instance.currentLevel)
            );

        if (btnLoadGame != null)
            btnLoadGame.onClick.AddListener(() => 
                GameManager.instance.LoadLevel(GameManager.instance.currentLevel)
            );

        if (btnExit_Yes != null)
            btnExit_Yes.onClick.AddListener(() => 
                GameManager.instance.QuitGame()
            );

        if (btnExit_No != null)
            btnExit_No.onClick.AddListener(() => 
                Debug.Log("Cancelar salida")
            );
    }
}
