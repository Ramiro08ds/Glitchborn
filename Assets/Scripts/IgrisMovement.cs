using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class IgrisMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public Transform throne;

    private NavMeshAgent agent;
    private EnemyAnimatorController animatorController;

    [Header("Configuración")]
    public float detectionRange = 15f;
    public float stoppingDistance = 2.5f;
    public float standUpDuration = 2f;
    public float sitDownDuration = 1.5f;

    private bool isSitting = true;
    private bool isStandingUp = false;
    private bool isStunned = false;
    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorController = GetComponent<EnemyAnimatorController>();

        if (throne == null)
        {
            GameObject throneObj = GameObject.Find("ElTrono");
            if (throneObj != null) throne = throneObj.transform;
        }

        agent.isStopped = true;
        isSitting = true;
    }

    void Update()
    {
        if (isStunned || isSitting) return;

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            isMoving = true;
        }
        else
        {
            agent.isStopped = true;
            isMoving = false;
        }

        // Volver al trono si el jugador se aleja demasiado
        if (distance > detectionRange * 2 && !isSitting && !isStandingUp)
        {
            StartCoroutine(SitDownSequence());
        }
    }

    public bool IsMoving() => isMoving;
    public bool IsSitting() => isSitting;

    public void SetStunned(bool state)
    {
        isStunned = state;
        agent.isStopped = state;
        isMoving = !state;
    }

    public void StandUp()
    {
        if (isStandingUp || !isSitting) return;
        StartCoroutine(StandUpSequence());
    }

    IEnumerator StandUpSequence()
    {
        isStandingUp = true;
        agent.isStopped = true;
        animatorController.PlayStandUp();
        yield return new WaitForSeconds(standUpDuration);

        isSitting = false;
        isStandingUp = false;
        agent.isStopped = false;
    }

    IEnumerator SitDownSequence()
    {
        isStandingUp = true;
        agent.isStopped = true;
        isMoving = false;
        isSitting = true;

        yield return new WaitForSeconds(sitDownDuration);

        if (throne != null) agent.Warp(throne.position);
        isStandingUp = false;
        agent.isStopped = true;
    }
}
