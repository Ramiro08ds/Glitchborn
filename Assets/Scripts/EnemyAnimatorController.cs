using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
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

        if (animator == null)
            Debug.LogWarning("⚠️ No se encontró un Animator en Igris.");
        if (agent == null)
            Debug.LogWarning("⚠️ No se encontró NavMeshAgent en Igris.");
    }

    void Update()
    {
        if (isDead || animator == null) return;

        // ✅ Estado de aturdimiento
        bool isStunned = igrisMovement != null && igrisMovement.IsStunned();
        animator.SetBool("IsStunned", isStunned);
        if (isStunned) return;

        // ✅ Movimiento: se basa en si el agente está realmente moviéndose
        bool isMoving = !agent.isStopped && agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("IsMoving", isMoving);

        // ✅ Jugador en rango (solo si hay referencia)
        if (igrisMovement != null && igrisAttack != null && igrisMovement.target != null)
        {
            float distance = Vector3.Distance(transform.position, igrisMovement.target.position);
            bool playerInRange = distance <= igrisAttack.attackRange;
            animator.SetBool("PlayerInRange", playerInRange);
        }
    }

    public void PlayDeathAnimation()
    {
        if (isDead || animator == null) return;

        isDead = true;
        animator.SetTrigger("Die");

        // Desactivar movimiento
        if (agent != null)
            agent.isStopped = true;

        // Opcional: desactivar colisiones y ataques
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        if (igrisAttack != null)
            igrisAttack.enabled = false;

        Debug.Log("💀 Igris ha muerto: animación de muerte reproducida.");
    }
}
