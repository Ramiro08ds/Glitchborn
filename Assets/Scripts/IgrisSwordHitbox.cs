using UnityEngine;

public class IgrisSwordHitbox : MonoBehaviour
{
    private int damage = 10;

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Igris golpeó al jugador por {damage} de daño.");
            }
        }
    }
}
