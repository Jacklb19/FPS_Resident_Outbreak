using UnityEngine;

public class Level3_GraveyardFlow : LevelFlow
{
    [Header("Cementerio - Generadores")]
    public int generatorsRequired = 3;
    private int generatorsActivated = 0;

    [Header("Boss")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    private BossHealth bossInstance;
    private bool bossSpawned = false;

    protected override void Setup()
    {
        objectivesRequired = generatorsRequired;

        var generators = FindObjectsOfType<GeneratorSwitch>();
        foreach (var gen in generators)
        {
            gen.onActivated += OnGeneratorActivated;
        }

        Debug.Log($"[Graveyard] {generators.Length} generadores encontrados. Objetivo: {generatorsRequired}");
    }

    private void OnGeneratorActivated(GeneratorSwitch generator)
    {
        generatorsActivated++;
        objectivesCompleted = generatorsActivated;

        Debug.Log($"[Graveyard] Generador activado: {generatorsActivated}/{generatorsRequired}");

        // Puntos por activar generador
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(200);
        }

        // Spawns de respuesta (Crawlers, etc.)
        if (spawnManager != null && generator.spawnOnActivate != null)
        {
            spawnManager.SpawnEntryNow(generator.spawnOnActivate);
        }

        if (generatorsActivated >= generatorsRequired && !bossSpawned)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        bossSpawned = true;

        if (bossPrefab != null && bossSpawnPoint != null)
        {
            GameObject bossObj = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            bossInstance = bossObj.GetComponent<BossHealth>();

            if (bossInstance != null)
            {
                bossInstance.OnBossDied += OnBossDefeated;
                Debug.Log("[Graveyard] Boss invocado - ¡Derrótalo para desbloquear la salida!");
            }
        }
        else
        {
            Debug.LogError("[Graveyard] Boss prefab o spawn point no asignado");
        }
    }

    private void OnBossDefeated()
    {
        Debug.Log("[Graveyard] Boss derrotado - nivel completado");

        // Puntos por derrotar boss
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(5000);
        }

        UnlockExit();
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

            Debug.Log("[Graveyard] Salida desbloqueada y outline activado");
        }

        Debug.Log("[Graveyard] Objetivo completado - Dirígete a la salida");
    }

    // ✅ NUEVO: método público llamado desde ExitTriggerZone
    public void OnPlayerReachedExit()
    {
        if (!exitUnlocked)
        {
            Debug.LogWarning("[Graveyard] Salida aún bloqueada");
            return;
        }

        Debug.Log("[Graveyard] Jugador alcanzó la salida → cargando pantalla de resultados");
        
        // Nivel 3 va a pantalla de resultados en lugar de siguiente nivel
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadResults();
        }
    }

    private void OnDestroy()
    {
        var generators = FindObjectsOfType<GeneratorSwitch>();
        foreach (var gen in generators)
        {
            gen.onActivated -= OnGeneratorActivated;
        }

        if (bossInstance != null)
        {
            bossInstance.OnBossDied -= OnBossDefeated;
        }
    }
}
