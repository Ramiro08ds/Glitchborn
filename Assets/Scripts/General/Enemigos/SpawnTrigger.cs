using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [Tooltip("Debe coincidir con el nombre de la zona en EnemySpawner")]
    public string zoneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (EnemySpawner.Instance == null)
            {
                Debug.LogError("❌ EnemySpawner.Instance es null. Asegúrate de tener uno en la escena.");
                return;
            }

            Debug.Log($"▶ Jugador entró a la zona {zoneName}, generando enemigos...");
            EnemySpawner.Instance.SpawnEnemies(zoneName);
            Destroy(gameObject); // Evita que se active más de una vez
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Collider col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
