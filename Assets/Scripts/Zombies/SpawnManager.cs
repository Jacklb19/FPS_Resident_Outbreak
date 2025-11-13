using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [Header("Config")]
    public WaveConfig config;
    public Transform[] defaultSpawnPoints;
    public int prewarmPerType = 8;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnEnemyCountChanged;

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    private int aliveCount = 0;
    private bool started = false;

    void Start()
    {
        // Fallback: si GameManager no llamó Begin(), arranca aquí
        if (!started && config != null)
        {
            Debug.Log("[SpawnManager] Fallback Start llamando Begin()");
            Begin();
        }
    }

    public void Begin()
    {
        if (started) return;
        started = true;

        if (config == null)
        {
            Debug.LogWarning("[SpawnManager] sin WaveConfig.");
            return;
        }

        Debug.Log($"[SpawnManager] Begin | timed:{config.timedSpawner} wave:{config.waveSpawner} event:{config.eventSpawner}");
        Debug.Log($"[SpawnManager] SpawnPoints: {(defaultSpawnPoints != null ? defaultSpawnPoints.Length : 0)}");

        if (config.timedSpawner && (config.timedEntries == null || config.timedEntries.Count == 0))
            Debug.LogWarning("[SpawnManager] timedSpawner activo pero sin timedEntries.");

        if (config.waveSpawner && (config.waves == null || config.waves.Count == 0))
            Debug.LogWarning("[SpawnManager] waveSpawner activo pero sin waves.");

        if (config.timedSpawner) StartCoroutine(RunTimed());
        if (config.waveSpawner) StartCoroutine(RunWaves());
    }

    private ObjectPool<GameObject> PoolFor(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab),
                actionOnGet: (go) => go.SetActive(true),
                actionOnRelease: (go) => go.SetActive(false),
                actionOnDestroy: (go) => Destroy(go),
                collectionCheck: false,
                defaultCapacity: prewarmPerType,
                maxSize: 256
            );
            pools[prefab] = pool;

            for (int i = 0; i < prewarmPerType; i++)
            {
                var go = pool.Get();
                pool.Release(go);
            }
        }
        return pool;
    }

    private Transform PickSpawnPoint(Transform[] overridePoints)
    {
        var arr = (overridePoints != null && overridePoints.Length > 0) ? overridePoints : defaultSpawnPoints;

        if (arr == null || arr.Length == 0)
        {
            Debug.LogError("[SpawnManager] ❌ No hay spawn points configurados");
            return null;
        }

        Debug.Log($"[SpawnManager] Usando {arr.Length} spawn points");

        // Si solo hay 1 y tiene hijos, usa los hijos
        if (arr.Length == 1 && arr[0] != null && arr[0].childCount > 0)
        {
            var parent = arr[0];
            int idx = UnityEngine.Random.Range(0, parent.childCount);
            var child = parent.GetChild(idx);
            Debug.Log($"[SpawnManager] ✅ Spawn point hijo: {child.name} | Posición: {child.position}");
            return child;
        }

        var selected = arr[UnityEngine.Random.Range(0, arr.Length)];
        Debug.Log($"[SpawnManager] ✅ Spawn point: {selected.name} | Posición: {selected.position}");
        return selected;
    }


    private void SpawnOne(GameObject prefab, Transform[] overridePoints = null)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[SpawnManager] Prefab nulo.");
            return;
        }

        var pool = PoolFor(prefab);
        var go = pool.Get();
        var navAgent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();

        var p = PickSpawnPoint(overridePoints);
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (p != null)
        {
            spawnPosition = p.position;
            spawnRotation = p.rotation;
        }
        else
        {
            Debug.LogWarning("[SpawnManager] Sin spawn point válido, usando origen.");
            spawnPosition = Vector3.zero;
            spawnRotation = Quaternion.identity;
        }

        if (navAgent != null)
        {
            navAgent.enabled = true; // ✅ HABILITAR PRIMERO
            navAgent.Warp(spawnPosition);
            go.transform.rotation = spawnRotation;
        }
        else
        {
            go.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        }

        // ✅ RESETEAR DESPUÉS de posicionar y habilitar NavMesh
        var zombie = go.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.enabled = true; // ✅ RE-HABILITAR el script
            zombie.ResetZombie();
        }

        var zh = go.GetComponent<ZombieHealth>();
        if (zh != null)
        {
            aliveCount++;
            OnEnemyCountChanged?.Invoke(aliveCount);

            void OnDeath()
            {
                zh.OnDied -= OnDeath;
                aliveCount--;
                OnEnemyCountChanged?.Invoke(aliveCount);
                StartCoroutine(ReleaseAfterDelay(go, pool, 3f));
            }

            zh.OnDied += OnDeath;
            zh.ResetHealth();
        }
    }


    private IEnumerator ReleaseAfterDelay(GameObject go, ObjectPool<GameObject> pool, float delay)
    {
        yield return new WaitForSeconds(delay);
        pool.Release(go);
    }

    private IEnumerator SpawnGroup(WaveConfig.EnemyEntry e)
    {
        int remaining = e.count;
        while (remaining > 0)
        {
            int toSpawn = Mathf.Min(e.groupSize, remaining);
            for (int i = 0; i < toSpawn; i++) SpawnOne(e.prefab, e.spawnPointList.spawnPoints);
            remaining -= toSpawn;
            yield return new WaitForSeconds(config.timeBetweenGroups);
        }
    }

    private IEnumerator RunTimed()
    {
        yield return new WaitForSeconds(config.initialDelay);
        if (config.timedEntries == null) yield break;

        foreach (var e in config.timedEntries)
            StartCoroutine(TimedLoop(e));
    }

    private IEnumerator TimedLoop(WaveConfig.EnemyEntry e)
    {
        float period = Mathf.Max(0.05f, 1f / Mathf.Max(0.0001f, e.spawnRate));
        int spawned = 0;
        bool infinite = e.count <= 0 || e.count >= 999;

        Debug.Log($"[SpawnManager] TimedLoop {e.prefab.name} | period:{period}s infinite:{infinite}");

        while (infinite || spawned < e.count)
        {
            for (int i = 0; i < e.groupSize; i++)
            {
                if (!infinite && spawned >= e.count) break;
                SpawnOne(e.prefab, e.spawnPointList.spawnPoints);
                spawned++;
            }
            yield return new WaitForSeconds(period);
        }
    }

    private IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(config.initialDelay);
        if (config.waves == null || config.waves.Count == 0) yield break;

        for (int w = 0; w < config.waves.Count; w++)
        {
            OnWaveStarted?.Invoke(w + 1);

            var wave = config.waves[w];
            if (wave != null && wave.enemies != null)
            {
                foreach (var e in wave.enemies)
                {
                    if (e == null || e.prefab == null || e.count <= 0) continue;
                    StartCoroutine(SpawnGroup(e));
                }
            }


            yield return new WaitUntil(() => aliveCount == 0);


            if (w < config.waves.Count - 1)
            {
                yield return new WaitForSeconds(5f);
            }
        }
    }



    public void SpawnEntryNow(WaveConfig.EnemyEntry e)
    {
        if (e == null || e.prefab == null || e.count <= 0) return;
        StartCoroutine(SpawnGroup(e));
    }

    public void SpawnPrefabNow(GameObject prefab, int count, int groupSize = 1, Transform[] points = null)
    {
        if (prefab == null || count <= 0) return;

        var entry = new WaveConfig.EnemyEntry
        {
            prefab = prefab,
            count = count,
            groupSize = Mathf.Max(1, groupSize),
            spawnPointList = new WaveConfig.SpawnPointList { spawnPoints = points }
        };

        StartCoroutine(SpawnGroup(entry));
    }

}
