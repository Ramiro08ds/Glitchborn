using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public float attackRange = 2f;       // rango de alcance del golpe
    public float attackCooldown = 1.5f; // tiempo entre golpes
    public int attackDamage = 20;       // daño del golpe
    public LayerMask enemyLayer;        // capa de los enemigos

    private bool canAttack = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) // click izquierdo
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;

        // Detectar enemigos cerca en un radio
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            // Ver si el enemigo tiene un script de vida
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }

        // Reiniciar cooldown
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }


    void OnDrawGizmosSelected()
    {
        // Dibuja el rango en la escena para ver hasta dónde llega el golpe
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, attackRange);
    }
}