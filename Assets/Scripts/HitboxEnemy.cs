using UnityEngine;

public class HitboxEnemy : MonoBehaviour
{
    [Header("Referencia al EnemyHealth del padre")]
    public EnemyHealth enemyHealth;

    [Header("Multiplicador de daño (opcional)")]
    public float damageMultiplier = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            if (enemyHealth != null)
            {
                int damage = Mathf.RoundToInt(10 * damageMultiplier);
                enemyHealth.TakeDamage(damage);
                Debug.Log("HitboxEnemy recibió golpe por " + damage);
            }
        }
    }
}
