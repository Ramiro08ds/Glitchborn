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
    private IgrisHealth health; // NUEVO

    [Header("Attack Settings")]
    public float detectionRange = 8f;
    public float attackRange = 3f;
    public float attackCooldown = 2f;

    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        health = GetComponent<IgrisHealth>(); // NUEVO
    }

    void Update()
    {
        if (player == null) return;

        // 🚫 No puede mover ni atacar si está stuneado o muerto
        if (health != null && (health.isStunned || health.isDead))
        {
            agent.isStopped = true;
            return;
        }

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

        // 🚫 Si queda stuneado justo antes del ataque, cancelar
        if (health != null && (health.isStunned || health.isDead))
        {
            isAttacking = false;
            yield break;
        }

        // Elegir ataque aleatorio
        int random = Random.Range(0, 10);
        bool strongAttack = random < 3;

        if (strongAttack)
            animator.SetTrigger("AttackStrong");
        else
            animator.SetTrigger("AttackNormal");

        // Tiempo antes del golpe
        yield return new WaitForSeconds(0.45f);

        // 🚫 Evita activar hitbox si queda stuneado en medio del ataque
        if (health != null && (health.isStunned || health.isDead))
        {
            isAttacking = false;
            yield break;
        }

        swordHitbox.EnableHitbox();

        // Duración del golpe
        yield return new WaitForSeconds(0.55f);
        swordHitbox.DisableHitbox();

        yield return new WaitForSeconds(0.2f);

        isAttacking = false;
    }
}
