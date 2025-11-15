using UnityEngine;
using TMPro;
using System.Collections;

public class MissionUI : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI missionTitleText;
    public TextMeshProUGUI missionObjectiveText;

    private Level1_HospitalFlow hospitalFlow;
    private Level2_StreetFlow streetFlow;
    private Level3_GraveyardFlow graveyardFlow;

    void Start()
    {
        hospitalFlow = FindObjectOfType<Level1_HospitalFlow>();
        streetFlow = FindObjectOfType<Level2_StreetFlow>();
        graveyardFlow = FindObjectOfType<Level3_GraveyardFlow>();

        if (hospitalFlow != null)
        {
            InitHospitalMission();
        }
        else if (streetFlow != null)
        {
            InitStreetMission();
        }
        else if (graveyardFlow != null)
        {
            InitGraveyardMission();
        }
    }

    void InitHospitalMission()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = "Mision: Hospital";
        }

        UpdateHospitalObjective(0, hospitalFlow.packagesRequired);

        var pickups = FindObjectsOfType<MedicalPickup>();
        foreach (var pickup in pickups)
        {
            pickup.onCollected += OnHospitalPackageCollected;
        }
    }

    private int hospitalPackagesCollected = 0;
    void OnHospitalPackageCollected()
    {
        hospitalPackagesCollected++;

        if (hospitalPackagesCollected >= hospitalFlow.packagesRequired)
        {
            if (missionObjectiveText != null)
            {
                missionObjectiveText.text = "✓ ¡Ve a la salida marcada!";
                missionObjectiveText.color = Color.green;
            }
        }
        else
        {
            UpdateHospitalObjective(hospitalPackagesCollected, hospitalFlow.packagesRequired);
        }
    }

    void UpdateHospitalObjective(int current, int total)
    {
        if (missionObjectiveText != null)
        {
            missionObjectiveText.text = $"Recoger paquetes médicos: {current}/{total}";
        }
    }

    void InitStreetMission()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = "Mision: Sobrevive a las oleadas";
        }

        UpdateStreetObjective(0, 3, 0);

        if (streetFlow != null && streetFlow.spawnManager != null)
        {
            streetFlow.spawnManager.OnWaveStarted += OnStreetWaveStarted;
            streetFlow.spawnManager.OnEnemyCountChanged += OnStreetEnemyCountChanged;
        }
    }

    private int currentWave = 0;
    private int aliveEnemies = 0;

    void OnStreetWaveStarted(int wave)
    {
        currentWave = wave;
        UpdateStreetObjective(currentWave, 3, aliveEnemies);
    }

    void OnStreetEnemyCountChanged(int count)
    {
        aliveEnemies = count;

        if (currentWave >= 3 && aliveEnemies == 0)
        {
            if (missionObjectiveText != null)
            {
                missionObjectiveText.text = "✓ ¡Oleadas completadas! Ve a la salida";
                missionObjectiveText.color = Color.green;
            }
        }
        else
        {
            UpdateStreetObjective(currentWave, 3, aliveEnemies);
        }
    }

    void UpdateStreetObjective(int wave, int totalWaves, int alive)
    {
        if (missionObjectiveText != null)
        {
            missionObjectiveText.text = $"Oleada: {wave}/{totalWaves}\nEnemigos restantes: {alive}";
        }
    }

    void InitGraveyardMission()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = "Mision: Activa Generadores";
        }

        UpdateGraveyardObjective(0, 3, false);

        var generators = FindObjectsOfType<GeneratorSwitch>();
        foreach (var gen in generators)
        {
            gen.onActivated += OnGeneratorActivated;
        }
    }

    private int generatorsActivated = 0;
    private bool bossDefeated = false;

    void OnGeneratorActivated(GeneratorSwitch gen)
    {
        generatorsActivated++;

        if (generatorsActivated >= 3 && !bossDefeated)
        {
            if (missionObjectiveText != null)
            {
                missionObjectiveText.text = "✓ ¡Derrota al Boss!";
                missionObjectiveText.color = Color.yellow;
            }

            StartCoroutine(FindAndSubscribeToBoss());
        }
        else
        {
            UpdateGraveyardObjective(generatorsActivated, 3, false);
        }
    }

    IEnumerator FindAndSubscribeToBoss()
    {
        yield return new WaitForSeconds(0.5f);

        var boss = FindObjectOfType<BossHealth>();
        if (boss != null)
        {
            boss.OnBossDied += OnBossDefeated;
            Debug.Log("[MissionUI] ✅ Suscrito al evento del boss correctamente");
        }
        else
        {
            Debug.LogWarning("[MissionUI] ❌ No se encontró BossHealth después de 0.5s");
        }
    }

    void OnBossDefeated()
    {
        Debug.Log("[MissionUI] ✅ OnBossDefeated llamado - actualizando UI");

        bossDefeated = true;

        if (missionObjectiveText != null)
        {
            missionObjectiveText.text = "✓ ¡Boss derrotado! Ve a la salida";
            missionObjectiveText.color = Color.green;
        }
        else
        {
            Debug.LogError("[MissionUI] ❌ missionObjectiveText es null");
        }
    }

    void UpdateGraveyardObjective(int current, int total, bool bossActive)
    {
        if (missionObjectiveText != null)
        {
            if (bossActive)
            {
                missionObjectiveText.text = "¡Derrota al Boss!";
            }
            else
            {
                missionObjectiveText.text = $"Activar generadores: {current}/{total}";
            }
        }
    }

    void OnDestroy()
    {
        if (streetFlow != null && streetFlow.spawnManager != null)
        {
            streetFlow.spawnManager.OnWaveStarted -= OnStreetWaveStarted;
            streetFlow.spawnManager.OnEnemyCountChanged -= OnStreetEnemyCountChanged;
        }

        var generators = FindObjectsOfType<GeneratorSwitch>();
        foreach (var gen in generators)
        {
            gen.onActivated -= OnGeneratorActivated;
        }

        var boss = FindObjectOfType<BossHealth>();
        if (boss != null)
        {
            boss.OnBossDied -= OnBossDefeated;
        }
    }
}
