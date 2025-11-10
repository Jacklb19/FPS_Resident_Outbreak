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
        public float spawnRate = 1f;       // unidades/seg
        public int groupSize = 1;          // tamaño de grupo
        public Transform[] spawnPoints;    // opcional: puntos específicos por tipo
    }

    [Header("Modo")]
    public bool timedSpawner = false;      // Nivel 1
    public bool waveSpawner = true;        // Nivel 2
    public bool eventSpawner = false;      // Nivel 3

    [Header("Timers")]
    public float initialDelay = 2f;
    public float timeBetweenGroups = 2f;   // entre grupos
    public float timeBetweenWaves = 90f;   // oleadas

    [Header("Oleadas")]
    public List<EnemyEntry>[] waves;       // Nivel 2: lista por oleada

    [Header("Spawns temporizados")]
    public List<EnemyEntry> timedEntries;  // Nivel 1: entradas globales temporizadas
}
