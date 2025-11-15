using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text breakdownText;
    public Button btnRestart;
    public Button btnMenu;

    void Start()
    {
        // Mostrar el cursor para UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Muestra la info del GameManager
        var gm = GameManager.instance;
        if (scoreText) scoreText.text = $"Score Final: {gm.totalScore}";
        if (timeText)
        {
            int minutes = Mathf.FloorToInt(gm.playTime / 60f);
            int seconds = Mathf.FloorToInt(gm.playTime % 60f);
            timeText.text = $"Tiempo: {minutes:00}:{seconds:00}";
        }

        if (breakdownText)
        {
            string breakdown =
                $"Zombies eliminados: {gm.zombiesKilled} x {gm.scorePerZombie} = <color=yellow>{gm.zombiesKilled * gm.scorePerZombie}</color>\n" +
                $"Generadores activados: {gm.generatorsActivated} x {gm.scorePerGenerator} = <color=yellow>{gm.generatorsActivated * gm.scorePerGenerator}</color>\n" +
                $"Boss derrotado: {gm.bossKilled} x {gm.scorePerBoss} = <color=yellow>{gm.bossKilled * gm.scorePerBoss}</color>\n" +
                $"Paquetes médicos: {gm.medicalPackagesCollected} x {gm.scorePerPackage} = <color=yellow>{gm.medicalPackagesCollected * gm.scorePerPackage}</color>\n" +
                $"Oleadas completadas: {gm.wavesCompleted} x {gm.scorePerWave} = <color=yellow>{gm.wavesCompleted * gm.scorePerWave}</color>\n";

            int mostrarMundos = 3;
            for (int i = 0; i < mostrarMundos; i++)
            {
                int score = 0;
                if (i < gm.worldScores.Count)
                    score = gm.worldScores[i];
                breakdown += $"Mundo {i + 1}: <b><color=#92caff>{score}</color></b>\n";
            }
            breakdownText.text = breakdown;
        }

        // Asignar listeners a los botones por código (importante para managers DontDestroyOnLoad)
        if (btnRestart != null)
        {
            btnRestart.onClick.RemoveAllListeners();
            btnRestart.onClick.AddListener(() => GameManager.instance.RestartLastLevel());
        }
        if (btnMenu != null)
        {
            btnMenu.onClick.RemoveAllListeners();
            btnMenu.onClick.AddListener(() => GameManager.instance.LoadMainMenu());
        }
    }
}
