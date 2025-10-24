using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.stoppingDistance = 0.1f; // que se pegue bien al player
        agente.updateRotation = true;

        // Si el agente aún no está en el NavMesh, espera hasta que esté listo
        StartCoroutine(WaitForNavMeshAndMove());
    }

    IEnumerator WaitForNavMeshAndMove()
    {
        // Espera hasta que el agente esté sobre el NavMesh
        yield return new WaitUntil(() => agente.isOnNavMesh);

        // Una vez listo, activa el movimiento
        agente.isStopped = false;
    }

    void Update()
    {
        // Solo mueve el agente si está activo y sobre el NavMesh
        if (destino != null && agente != null && agente.isOnNavMesh)
        {
            agente.SetDestination(destino.position);
        }
    }
}
