using UnityEngine;

public class Level2_StreetFlow : LevelFlow
{
    [Header("Calle - Oleadas")]
    public int totalWaves = 3;
    private int currentWave = 0;
    private int aliveEnemies = 0;
    private bool allWavesSpawned = false;

    protected override void Setup()
    {
        objectivesRequired = totalWaves;

        if (spawnManager != null)
        {
            spawnManager.OnWaveStarted += OnWaveStarted;
            spawnManager.OnEnemyCountChanged += OnEnemyCountChanged;
        }

        Debug.Log($"[Street] Iniciando sistema de oleadas. Total: {totalWaves}");
    }

    private void OnWaveStarted(int waveNumber)
    {
        currentWave = waveNumber;
        objectivesCompleted = currentWave;

        Debug.Log($"[Street] Oleada {currentWave}/{totalWaves} iniciada");

        if (currentWave >= totalWaves)
        {
            allWavesSpawned = true;
            Debug.Log("[Street] Última oleada spawneada - elimina a todos los enemigos");
        }
    }

    private void OnEnemyCountChanged(int count)
    {
        aliveEnemies = count;

        if (allWavesSpawned && aliveEnemies == 0)
        {
            Debug.Log("[Street] Todas las oleadas completadas y 0 enemigos vivos");
            UnlockExit();
        }
    }

    protected new void UnlockExit()
    {
        if (exitUnlocked) return;
        exitUnlocked = true;

        if (exitDoor != null)
        {
            exitDoor.SetActive(true);

            // Activar Outline de la puerta
            Outline[] outlines = exitDoor.GetComponentsInChildren<Outline>(true);
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }

            Debug.Log("[Street] Salida desbloqueada y outline activado");
        }

        // Bonus por completar nivel
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(1000);
        }

        Debug.Log("[Street] Objetivo completado - Dirígete a la salida");
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckExitTrigger(other);
    }

    private void OnDestroy()
    {
        if (spawnManager != null)
        {
            spawnManager.OnWaveStarted -= OnWaveStarted;
            spawnManager.OnEnemyCountChanged -= OnEnemyCountChanged;
        }
    }
}
