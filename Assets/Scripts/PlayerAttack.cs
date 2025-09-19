using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public LayerMask enemyLayer;

    [Header("Referencias")]
    public PlayerLevelUI menu; 
    private PlayerLevelSystem playerLevel; 

    private bool canAttack = true;

    void Start()
    {
  
        playerLevel = GetComponent<PlayerLevelSystem>();
    }

    void Update()
    {
        if (menu != null && menu.menuAbierto)
            return;

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;

        // Daño en base a la fuerza del PlayerLevelSystem
        int damage = 2 + playerLevel.strength * 1;

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}
