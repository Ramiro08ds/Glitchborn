using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            PlayerHealthManager.instance.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
