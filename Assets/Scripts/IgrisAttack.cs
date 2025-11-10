using UnityEngine;
using System.Collections;

public class IgrisAttack : MonoBehaviour
{
    public int normalDamage = 12;
    public int strongDamage = 20;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;

    public Transform attackPoint;
    public LayerMask playerLayer;

    private EnemyAnimatorController animatorController;
    private IgrisMovement movementScript;
    private bool canAttack = true;
    private int attackCount = 0;
    private bool playerInRange = false;

    void Start()
    {
        animatorController = GetComponent<EnemyAnimatorController>();
        movementScript = GetComponent<IgrisMovement>();

        if (attackPoint == null)
        {
            GameObject point = new GameObject("AttackPoint");
            point.transform.SetParent(transform);
            point.transform.localPosition = new Vector3(0, 1.5f, 1.5f);
            attackPoint = point.transform;
        }
    }

    void Update()
    {
        if (movementScript.IsSitting || movementScript.IsStunned || !canAttack) return;
        if (movementScript == null || animatorController == null) return;

        Transform target = movementScript.target;
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        playerInRange = distance <= attackRange;

        if (playerInRange && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    public bool IsPlayerInRange() => playerInRange;

    IEnumerator PerformAttack()
    {
        canAttack = false;
        attackCount++;

        movementScript.agent.isStopped = true;

        if (attackCount % 3 == 0)
            animatorController.PlayAttackStrong();
        else
            animatorController.PlayAttackNormal();

        yield return new WaitForSeconds(0.5f); // momento del impacto

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            PlayerHealthManager health = player.GetComponent<PlayerHealthManager>();
            if (health != null)
                health.TakeDamage(attackCount % 3 == 0 ? strongDamage : normalDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        movementScript.agent.isStopped = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
