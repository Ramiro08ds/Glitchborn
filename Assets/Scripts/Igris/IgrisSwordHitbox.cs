using UnityEngine;

public class IgrisSwordHitbox : MonoBehaviour
{
    private int damage = 10;
    private Collider col;
    private IgrisHealth igrisHealth; // NUEVO

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        // Obtener la salud para saber si está stuneado
        igrisHealth = GetComponentInParent<IgrisHealth>();
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
        if (!col.enabled) return;

        // 🚫 NUEVO: No hace daño si está stuneado o muerto
        if (igrisHealth != null && (igrisHealth.isStunned || igrisHealth.isDead))
            return;

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
