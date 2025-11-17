using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private IgrisMovement movement;
    private IgrisHealth health;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        movement = GetComponent<IgrisMovement>();
        health = GetComponent<IgrisHealth>();
    }

    void Update()
    {
        if (health.isDead) return;

        animator.SetBool("IsMoving", movement.IsMoving);
        animator.SetBool("IsSitting", movement.IsSitting);
        animator.SetBool("IsStunned", health.isStunned);
        animator.SetBool("PlayerInRange", movement.PlayerInRange);
    }

    public void PlayAttackNormal()
    {
        animator.SetTrigger("AttackNormal");
    }

    public void PlayAttackStrong()
    {
        animator.SetTrigger("AttackStrong");
    }

    public void PlayStandUp()
    {
        animator.SetTrigger("StandUp");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Die");
    }
}
