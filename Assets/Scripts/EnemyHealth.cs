using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public EnemyHealthBar healthBar; 

    void Start()
    {
        currentHealth = maxHealth;

       
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<EnemyHealthBar>();
            if (healthBar != null)
                Debug.Log("[EnemyHealth] HealthBar asignada automáticamente desde hijos.");
        }

       
        if (healthBar != null)
        {
            healthBar.target = transform;
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " recibió " + amount + " de daño. Vida restante: " + currentHealth);

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió!");
        Destroy(gameObject);
    }
}
