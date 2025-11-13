using UnityEngine;
using TMPro;

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
        // Detectar qué nivel estamos jugando y suscribirse
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

    // ========== NIVEL 1: HOSPITAL ==========
    void InitHospitalMission()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = "MISIÓN: HOSPITAL";
        }

        UpdateHospitalObjective(0, hospitalFlow.packagesRequired);

        // Suscribirse a cambios
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
            // Objetivo completado
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

    // ========== NIVEL 2: CALLE ==========
    void InitStreetMission()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = "MISIÓN: SOBREVIVE LAS OLEADAS";
        }

        UpdateStreetObjective(0, 3, 0);

        // Suscribirse a eventos de oleadas
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
            // Completado
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

    // ========== NIVEL 3: CEMENTERIO ==========
    void InitGraveyardMission()
    {
        if (missionTitleText != null)
        {
            missionTitleText.text = "MISIÓN: ACTIVA GENERADORES";
        }

        UpdateGraveyardObjective(0, 3, false);

        // Suscribirse a generadores
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

            // Suscribirse al boss
            if (graveyardFlow != null)
            {
                var boss = FindObjectOfType<BossHealth>();
                if (boss != null)
                {
                    boss.OnBossDied += OnBossDefeated;
                }
            }
        }
        else
        {
            UpdateGraveyardObjective(generatorsActivated, 3, false);
        }
    }

    void OnBossDefeated()
    {
        bossDefeated = true;
        if (missionObjectiveText != null)
        {
            missionObjectiveText.text = "✓ ¡Boss derrotado! Ve a la salida";
            missionObjectiveText.color = Color.green;
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
        // Desuscribirse de eventos
        if (streetFlow != null && streetFlow.spawnManager != null)
        {
            streetFlow.spawnManager.OnWaveStarted -= OnStreetWaveStarted;
            streetFlow.spawnManager.OnEnemyCountChanged -= OnStreetEnemyCountChanged;
        }
    }
}
