using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class IgrisMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public Transform throne;

    public NavMeshAgent agent;
    private EnemyAnimatorController animatorController;

    [Header("Configuración")]
    public float detectionRange = 15f;
    public float stoppingDistance = 2.5f;
    public float standUpDuration = 2f;
    public float sitDownDuration = 1.5f;

    private bool _isSitting = true;
    private bool _isStandingUp = false;
    private bool _isStunned = false;
    private bool _isMoving = false;


    #region Public API 

    public bool IsStunned => _isStunned;
    public bool IsStandingUp => _isStandingUp;
    public bool IsMoving => _isMoving;
    public bool IsSitting => _isSitting;

    #endregion

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
        _isSitting = true;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (!ValidateBehavior(distance)) {
            _isMoving = false;
        };


        agent.SetDestination(target.position);
        _isMoving = true;

        // Volver al trono si el jugador se aleja demasiado
        if (distance > detectionRange * 2 && !_isSitting && !_isStandingUp)
        {
            StartCoroutine(SitDownSequence());
        }
    }

    public void SetStunned(bool state)
    {
        _isStunned = state;
        agent.isStopped = state;
        _isMoving = !state;
    }

    private bool ValidateBehavior(float dist)
    {

        if (dist > detectionRange && !_isStunned) return true;
        if (_isStunned || _isSitting) return false;

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target == null) return false;

        return true;
    }

    public void StandUp()
    {
        if (_isStandingUp || !_isSitting) return;
        StartCoroutine(StandUpSequence());
    }

    IEnumerator StandUpSequence()
    {
        _isStandingUp = true;
        agent.isStopped = true;
        animatorController.PlayStandUp();
        yield return new WaitForSeconds(standUpDuration);

        _isSitting = false;
        _isStandingUp = false;
        agent.isStopped = false;
    }

    IEnumerator SitDownSequence()
    {
        _isStandingUp = true;
        agent.isStopped = true;
        _isMoving = false;
        _isSitting = true;

        yield return new WaitForSeconds(sitDownDuration);

        if (throne != null) agent.Warp(throne.position);
        _isStandingUp = false;
        agent.isStopped = true;
    }
}
