using UnityEngine;

public class IgrisSwordHitbox : MonoBehaviour
{
    private int damage = 10;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false; // IMPORTANTÍSIMO
    }

    public void EnableHitbox()
    {
        col.enabled = true;
    }

    public void DisableHitbox()
    {
        col.enabled = false;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!col.enabled) return; // seguridad

        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
