using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Game/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        public int count = 0;
        public float spawnRate = 1f;
        public int groupSize = 1;
        public Transform[] spawnPoints;
    }

    // ✅ NUEVO: Clase wrapper para serializar oleadas
    [Serializable]
    public class Wave
    {
        public List<EnemyEntry> enemies = new List<EnemyEntry>();
    }

    [Header("Modo")]
    public bool timedSpawner = false;
    public bool waveSpawner = true;
    public bool eventSpawner = false;

    [Header("Timers")]
    public float initialDelay = 2f;
    public float timeBetweenGroups = 2f;
    public float timeBetweenWaves = 90f;

    [Header("Oleadas")]
    public List<Wave> waves = new List<Wave>();  // ✅ Ahora Unity puede mostrarlo

    [Header("Spawns temporizados")]
    public List<EnemyEntry> timedEntries = new List<EnemyEntry>();
}
