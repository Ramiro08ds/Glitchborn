using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IgrisAttack : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform player;
    public IgrisSwordHitbox swordHitbox;

    [Header("Attack Settings")]
    public float detectionRange = 8f;
    public float attackRange = 3f;
    public float attackCooldown = 2f;

    private float lastAttackTime;
    private bool isAttacking = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Movimiento
        bool shouldMove = distance <= detectionRange && distance > attackRange;
        animator.SetBool("IsMoving", shouldMove);
        agent.isStopped = !shouldMove;

        // Ataque
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            StartCoroutine(DoAttack());
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator DoAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;

        // Elegir ataque aleatorio
        int random = Random.Range(0, 10); // 0–9
        bool strongAttack = random < 3;   // 30% fuerte, 70% normal

        if (strongAttack)
            animator.SetTrigger("AttackStrong");
        else
            animator.SetTrigger("AttackNormal");

        // Tiempo antes de golpear (ajustable)
        yield return new WaitForSeconds(0.45f); 
        swordHitbox.EnableHitbox();

        // Duración del golpe (ajustable)
        yield return new WaitForSeconds(0.55f); 
        swordHitbox.DisableHitbox();

        // Pequeña pausa antes de volver a atacar
        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }
}
