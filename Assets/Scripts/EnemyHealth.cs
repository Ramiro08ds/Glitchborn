using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public EnemyHealthBar healthBar; // referencia a la barra

    void Start()
    {
        currentHealth = maxHealth;

        // Si no está asignada la barra, la busca en los hijos
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<EnemyHealthBar>();
            if (healthBar != null)
                Debug.Log("[EnemyHealth] HealthBar asignada automáticamente desde hijos.");
        }

        // 👉 asignar target de la barra al enemigo
        if (healthBar != null)
        {
            healthBar.target = transform; // importante
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
