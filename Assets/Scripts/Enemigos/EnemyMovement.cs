using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public Transform destino;
    private NavMeshAgent agente;

    [Header("Animators")]
    public Animator cuerpoAnimator;   // CuerpoEnemyAnimator
    public Animator alasAnimator;     // AlasAnimator

    [Header("Flight Settings")]
    public bool canFly = false;
    public float flyingHeight = 5f;
    public float flightSmooth = 2f;
    public bool isFlying = false;

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
        if (canFly && isFlying)
            MaintainFlightHeight();

        HandleMovementAnimations();

        if (!isFlying && agente.isOnNavMesh && destino != null)
        {
            agente.SetDestination(destino.position);
        }
    }

    void MaintainFlightHeight()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, flyingHeight, Time.deltaTime * flightSmooth);
        transform.position = pos;
    }

    void HandleMovementAnimations()
    {
        if (!isFlying) return;

        // Si el enemigo está volando y moviéndose hacia un target
        if (destino != null)
        {
            float dist = Vector3.Distance(transform.position, destino.position);

            // Si está lejos y va hacia el player → movimiento
            if (dist > 2f)
                cuerpoAnimator.SetBool("isMoving", true);
            else
                cuerpoAnimator.SetBool("isMoving", false);
        }
    }

    public void StartFlying()
    {
        if (isFlying) return;

        isFlying = true;
        agente.enabled = false;

        cuerpoAnimator.SetBool("isFlying", true);
        alasAnimator.SetBool("isFlying", true);
    }

    public void StopFlying()
    {
        if (!isFlying) return;

        isFlying = false;
        agente.enabled = true;

        cuerpoAnimator.SetBool("isFlying", false);
        alasAnimator.SetBool("isFlying", false);
        cuerpoAnimator.SetBool("isMoving", false);
    }

    public void SetTarget(Transform target)
    {
        destino = target;
    }
}
