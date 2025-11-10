using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private IgrisHealth igrisHealth;
    private IgrisAttack igrisAttack;
    private IgrisMovement igrisMovement;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        igrisHealth = GetComponent<IgrisHealth>();
        igrisAttack = GetComponent<IgrisAttack>();
        igrisMovement = GetComponent<IgrisMovement>();

        if (animator == null) Debug.LogWarning("⚠️ No se encontró Animator en Igris.");
        if (agent == null) Debug.LogWarning("⚠️ No se encontró NavMeshAgent en Igris.");
    }

    void Update()
    {
        if (isDead || animator == null) return;

        // 🔹 Estado de aturdimiento
        bool isStunned = igrisMovement != null && igrisMovement.IsStunned();
        animator.SetBool("IsStunned", isStunned);
        if (isStunned) return;

        // 🔹 Movimiento
        bool isMoving = igrisMovement != null && igrisMovement.IsMoving();
        animator.SetBool("IsMoving", isMoving);

        // 🔹 Jugador en rango para atacar
        bool playerInRange = igrisAttack != null && igrisAttack.IsPlayerInRange();
        animator.SetBool("PlayerInRange", playerInRange);

        // 🔹 Sentado o de pie
        bool isSitting = igrisMovement != null && igrisMovement.IsSitting();
        animator.SetBool("IsSitting", isSitting);
    }

    public void PlayAttackNormal() => animator.SetTrigger("AttackNormal");
    public void PlayAttackStrong() => animator.SetTrigger("AttackStrong");

    public void PlayStandUp() => animator.SetTrigger("StandUp");

    public void PlayDeath()
    {
        if (isDead || animator == null) return;

        isDead = true;
        animator.SetTrigger("Die");
        if (agent != null) agent.isStopped = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (igrisAttack != null) igrisAttack.enabled = false;
        if (igrisMovement != null) igrisMovement.enabled = false;
    }
}
