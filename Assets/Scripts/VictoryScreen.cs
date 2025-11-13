using UnityEngine;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text breakdownText; // Será usado para todo el resumen

    void Start()
    {
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
    }
}
