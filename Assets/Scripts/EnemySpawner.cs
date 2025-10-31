using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance; // Singleton

    [System.Serializable]
    public class SpawnPoint
    {
        public string zoneName;                  // Nombre de la zona
        public Transform[] spawnPositions;       // Puntos donde aparecerán los enemigos
        public GameObject[] enemyPrefabs;        // Prefabs posibles de enemigos
    }

    [Header("Configuración de zonas y enemigos")]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private void Awake()
    {
        // Garantiza que solo haya un EnemySpawner en la escena
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Spawnea enemigos de la zona indicada
    public void SpawnEnemies(string zoneName)
    {
        SpawnPoint zone = spawnPoints.Find(z => z.zoneName == zoneName);
        if (zone == null)
        {
            Debug.LogWarning($"No se encontró una zona llamada {zoneName}");
            return;
        }

        foreach (Transform pos in zone.spawnPositions)
        {
            int randomIndex = Random.Range(0, zone.enemyPrefabs.Length);
            GameObject enemyPrefab = zone.enemyPrefabs[randomIndex];
            Instantiate(enemyPrefab, pos.position, pos.rotation);
        }

        Debug.Log($"Enemigos spawneados en la zona: {zoneName}");
    }
}
