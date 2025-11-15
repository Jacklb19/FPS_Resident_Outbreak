using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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

        if (GameManager.instance != null)
        {
            GameManager.instance.AddGeneratorActivated();
        }

        // Aquí el cambio (itera sobre la lista de EnemyEntry)
        if (spawnManager != null && generator.spawnOnActivate != null)
        {
            foreach (var entry in generator.spawnOnActivate)
            {
                if (entry != null && entry.prefab != null && entry.count > 0)
                    spawnManager.SpawnEntryNow(entry);
            }
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

            NavMeshAgent bossAgent = bossObj.GetComponent<NavMeshAgent>();
            if (bossAgent != null)
            {
                bossAgent.enabled = false;
                StartCoroutine(EnableBossNavMesh(bossAgent));
            }

            bossInstance = bossObj.GetComponent<BossHealth>();

            if (bossInstance != null)
            {
                bossInstance.OnBossDied += OnBossDefeated;
                Debug.Log("[Graveyard] Boss invocado");
            }
        }
        else
        {
            Debug.LogError("[Graveyard] Boss prefab o spawn point no asignado");
        }
    }

    IEnumerator EnableBossNavMesh(NavMeshAgent agent)
    {
        yield return new WaitForSeconds(0.1f);
        if (agent != null)
        {
            agent.enabled = true;
        }
    }

    private void OnBossDefeated()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddBossKill();
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

            Outline[] outlines = exitDoor.GetComponentsInChildren<Outline>(true);
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }

            Debug.Log("[Graveyard] Salida desbloqueada");
        }
    }

    public void OnPlayerReachedExit()
    {
        if (!exitUnlocked)
        {
            Debug.LogWarning("[Graveyard] Salida aún bloqueada");
            return;
        }

        Debug.Log("[Graveyard] Jugador alcanzó la salida");

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
