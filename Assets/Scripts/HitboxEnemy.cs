using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxEnemy : MonoBehaviour
{
    [Header("Referencia al EnemyHealth del padre")]
    public EnemyHealth enemyHealth;

    [Header("Multiplicador de daño (opcional)")]
    public float damageMultiplier = 1f;

    // Detecta colisiones con el ataque del jugador
    void OnCollisionEnter(Collision collision)
    {
        // Si el objeto tiene el tag del arma del jugador
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            if (enemyHealth != null)
            {
                // Daño base multiplicado por el multiplicador
                int damage = Mathf.RoundToInt(10 * damageMultiplier);
                enemyHealth.TakeDamage(damage);
            }
        }
    }
}
