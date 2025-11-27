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

        // DEBUG: ver valor real en cada frame (comentar si spam)
        // Debug.Log($"[AnimCtrl] IsStunned param = {animator.GetBool(\"IsStunned\")}");
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

    // Fallback: forzar entrada a Stun si por alguna razón la transición no ocurre.
    // Requiere que tengas un estado en el Animator llamado EXACTAMENTE "Stun" (cambiar si es otro nombre).
    public void ForceEnterStun()
    {
        if (animator == null) return;

        Debug.Log("[AnimCtrl] ForceEnterStun() called — seteando param IsStunned = true");
        animator.SetBool("IsStunned", true);

        // Intenta forzar el state directamente (fallback). REVISAR: el nombre del state debe ser EXACTO.
        // Si tu estado se llama distinto, cambialo por el nombre correcto.
        try
        {
            animator.Play("Stun");
            Debug.Log("[AnimCtrl] animator.Play(\"Stun\") ejecutado como fallback.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[AnimCtrl] animator.Play(\"Stun\") falló: " + ex.Message);
        }
    }

    public void ForceExitStun()
    {
        if (animator == null) return;

        Debug.Log("[AnimCtrl] ForceExitStun() called — seteando param IsStunned = false");
        animator.SetBool("IsStunned", false);

        // No forzamos otro estado aquí; el Animator debería transicionar según sus reglas.
    }
}
