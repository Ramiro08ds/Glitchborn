using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class IgrisMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public Transform throne;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Configuración")]
    public float detectionRange = 15f;
    public float stoppingDistance = 2.5f;
    public float standUpDuration = 2f;

    private bool isSitting = true;
    private bool isStunned = false;
    private bool isStandingUp = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Buscar el trono automáticamente si no está asignado
        if (throne == null)
        {
            GameObject throneObj = GameObject.Find("ElTrono");
            if (throneObj != null)
                throne = throneObj.transform;
        }

        // Estado inicial
        agent.isStopped = true;
        animator.SetBool("IsSitting", true);
        animator.SetBool("IsMoving", false);
        animator.SetBool("PlayerInRange", false);
        animator.SetBool("IsStunned", false);
    }

    void Update()
    {
        if (isStunned) return;

        // Buscar player si no lo tiene
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }

        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Si está sentado y el jugador se acerca, se levanta
        if (isSitting && distance < detectionRange && !isStandingUp)
        {
            StartCoroutine(StandUpSequence());
        }

        // Si ya está de pie, se mueve o ataca
        if (!isSitting && !isStunned)
        {
            MoveTowardPlayer(distance);
        }
    }

    void MoveTowardPlayer(float distance)
    {
        if (distance > stoppingDistance)
        {
            // Se mueve hacia el jugador
            agent.isStopped = false;
            agent.SetDestination(target.position);

            animator.SetBool("IsMoving", true);
            animator.SetBool("PlayerInRange", false);
        }
        else
        {
            // Llega al rango de ataque
            agent.isStopped = true;

            animator.SetBool("IsMoving", false);
            animator.SetBool("PlayerInRange", true);
        }

        // Si el jugador se aleja mucho, vuelve al trono
        if (distance > detectionRange * 2 && !isSitting && !isStandingUp)
        {
            StartCoroutine(SitDownSequence());
        }
    }

    IEnumerator StandUpSequence()
    {
        isStandingUp = true;
        agent.isStopped = true;

        animator.SetBool("IsSitting", false);
        animator.SetTrigger("StandUp");

        yield return new WaitForSeconds(standUpDuration);

        isSitting = false;
        isStandingUp = false;
        agent.isStopped = false;
    }

    IEnumerator SitDownSequence()
    {
        isStandingUp = true;
        agent.isStopped = true;

        animator.SetBool("IsMoving", false);
        animator.SetBool("PlayerInRange", false);
        animator.SetBool("IsSitting", true);

        yield return new WaitForSeconds(1.5f);

        isSitting = true;
        isStandingUp = false;
        agent.isStopped = true;

        // Teletransporta de vuelta al trono (opcional)
        if (throne != null)
            agent.Warp(throne.position);
    }

    // 🔥 Llamado por IgrisHealth cuando baja a cierta vida o recibe aturdimiento
    public void SetStunned(bool state)
    {
        isStunned = state;
        agent.isStopped = state;
        animator.SetBool("IsStunned", state);

        if (state)
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("PlayerInRange", false);
        }
    }

    // 🔥 Llamado por IgrisHealth cuando muere
    public void Die()
    {
        agent.isStopped = true;
        animator.SetTrigger("Die");
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
