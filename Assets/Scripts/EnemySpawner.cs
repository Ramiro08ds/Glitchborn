using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [System.Serializable]
    public class SpawnPoint
    {
        public string zoneName;
        public Transform[] spawnPositions;
        public GameObject[] enemyPrefabs;
    }

    [Header("Configuración de zonas y enemigos")]
    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [Header("Referencia al Player")]
    public PlayerLevelSystem player;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnEnemies(string zoneName)
    {
        if (spawnPoints == null || spawnPoints.Count == 0) return;

        SpawnPoint zone = spawnPoints.Find(z => z.zoneName == zoneName);
        if (zone == null) return;

        foreach (Transform pos in zone.spawnPositions)
        {
            if (pos == null) continue;

            List<GameObject> validPrefabs = new List<GameObject>();
            foreach (GameObject prefab in zone.enemyPrefabs)
                if (prefab != null) validPrefabs.Add(prefab);

            if (validPrefabs.Count == 0) return;

            GameObject enemyPrefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
            GameObject enemy = Instantiate(enemyPrefab, pos.position, pos.rotation);

            // Asignamos player como target y referencia para XP
            EnemyMovement em = enemy.GetComponent<EnemyMovement>();
            if (em != null && player != null)
                em.SetTarget(player.transform);

            EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
            if (eh != null && player != null)
                eh.SetPlayer(player);
        }
    }
}
