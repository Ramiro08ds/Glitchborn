using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Rewards")]
    public int xpReward = 1;   
    public int healOnKill = 20;

    [Header("References")]
    public EnemyHealthBar healthBar;
    public PlayerLevelSystem player;

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

        // Dar XP + curar al player
        if (player != null)
        {
            player.EnemyKilled(xpReward, healOnKill);
        }
        else
        {
            Debug.LogWarning("[EnemyHealth] No se asignó PlayerLevelSystem. No se dará XP/curación.");
        }

        // Destruir al enemigo
        Destroy(gameObject);
    }
}
