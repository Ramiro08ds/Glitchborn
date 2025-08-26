using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.stoppingDistance = 0.1f; // que se pegue bien al player
        agente.updateRotation = true;  
    }

    void Update()
    {
        if (destino != null)
        {
            agente.SetDestination(destino.position);
        }
    }
}
