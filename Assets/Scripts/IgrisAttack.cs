using UnityEngine;
using System.Collections;

public class IgrisAttack : MonoBehaviour
{
    [Header("Ataque")]
    public int normalDamage = 12;
    public int strongDamage = 20;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;

    [Tooltip("Punto desde donde se calcula el alcance del ataque")]
    public Transform attackPoint;

    [Tooltip("Capa del jugador")]
    public LayerMask playerLayer;

    [Header("Referencias")]
    public Animator animator;
    public IgrisMovement movementScript;

    private bool canAttack = true;
    private int attackCount = 0;
    public bool playerInRange { get; private set; }

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (movementScript == null)
            movementScript = GetComponent<IgrisMovement>();

        // ✅ Si no hay AttackPoint, crear uno automáticamente
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
        if (movementScript == null || movementScript.IsStunned()) return;

        if (movementScript.target != null && canAttack)
        {
            float distance = Vector3.Distance(transform.position, movementScript.target.position);
            playerInRange = distance <= attackRange;

            animator.SetBool("PlayerInRange", playerInRange);

            if (playerInRange)
                StartCoroutine(PerformAttack());
        }
        else
        {
            playerInRange = false;
            animator.SetBool("PlayerInRange", false);
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;
        attackCount++;

        bool isStrongAttack = (attackCount % 3 == 0);

        // ✅ usa los triggers que vos definiste
        if (isStrongAttack)
            animator.SetTrigger("AttackStrong");
        else
            animator.SetTrigger("AttackNormal");

        yield return new WaitForSeconds(0.5f); // momento del impacto

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
        foreach (Collider player in hitPlayers)
        {
            PlayerHealthManager playerHealth = player.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                int dmg = isStrongAttack ? strongDamage : normalDamage;
                playerHealth.TakeDamage(dmg);
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
