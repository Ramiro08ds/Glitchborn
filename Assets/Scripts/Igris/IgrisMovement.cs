using UnityEngine;
using UnityEngine.AI;

public class IgrisMovement : MonoBehaviour
{
    public Transform target;

    public float detectionRange = 20f;
    public float attackRange = 3f;

    public bool IsMoving { get; private set; }
    public bool IsSitting { get; private set; } = true;
    public bool IsStunned { get; private set; }
    public bool PlayerInRange { get; private set; }

    private NavMeshAgent agent;
    private EnemyAnimatorController animatorController;
    private IgrisHealth health;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorController = GetComponent<EnemyAnimatorController>();
        health = GetComponent<IgrisHealth>();

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        agent.isStopped = true;
    }

    void Update()
    {
        if (health.isDead) return;

        float distance = Vector3.Distance(transform.position, target.position);
        PlayerInRange = distance <= attackRange;

        if (IsSitting)
        {
            if (distance <= detectionRange)
            {
                StandUp();
            }
            return;
        }

        if (IsStunned) return;

        agent.SetDestination(target.position);
        agent.isStopped = false;

        IsMoving = agent.velocity.magnitude > 0.1f;
    }

    public void StandUp()
    {
        IsSitting = false;
        animatorController.PlayStandUp();
        Invoke("StartMoving", 2f);
    }

    void StartMoving()
    {
        agent.isStopped = false;
    }

    public void SetStunned(bool state)
    {
        IsStunned = state;
        agent.isStopped = state;
        IsMoving = !state;
    }
}
