using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform destino; // El Player como target
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.stoppingDistance = 0.1f;
        agente.updateRotation = true;

        StartCoroutine(WaitForNavMeshAndMove());
    }

    IEnumerator WaitForNavMeshAndMove()
    {
        yield return new WaitUntil(() => agente.isOnNavMesh);
        agente.isStopped = false;
    }

    void Update()
    {
        if (destino != null && agente != null && agente.isOnNavMesh)
        {
            agente.SetDestination(destino.position);
        }
    }

    // Método público para asignar destino desde el spawner
    public void SetTarget(Transform target)
    {
        destino = target;
    }
}
