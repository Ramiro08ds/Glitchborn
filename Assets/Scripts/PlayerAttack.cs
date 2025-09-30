using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public float attackRange = 1.2f;          // radio del golpe
    public float attackCooldown = 1.0f;       // tiempo entre ataques
    public float delayBeforeHit = 0.12f;      // delay para sincronizar con la animación
    public LayerMask enemyLayer;

    [Header("Referencias")]
    public Transform hitPoint;                // Empty en la punta de la espada
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
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // Espera un poco para que coincida con el frame del golpe
        yield return new WaitForSeconds(delayBeforeHit);

        // Daño en base a la fuerza del PlayerLevelSystem
        int damage = 2 + playerLevel.strength * 1;

        // Busca colliders en rango desde el punto de impacto
        Collider[] hitEnemies = Physics.OverlapSphere(hitPoint.position, attackRange, enemyLayer);

        // Evita golpear varias veces al mismo enemigo si tiene más de un collider
        HashSet<EnemyHealth> damaged = new HashSet<EnemyHealth>();

        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth health = enemy.GetComponentInParent<EnemyHealth>();
            if (health != null && !damaged.Contains(health))
            {
                health.TakeDamage(damage);
                damaged.Add(health);
                Debug.Log("Golpeé a: " + health.name + " por " + damage + " de daño");
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (hitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint.position, attackRange);
        }
    }
}
