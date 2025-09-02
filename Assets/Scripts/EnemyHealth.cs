using UnityEngine;
using UnityEngine.UI;


public class EnemyHealth : MonoBehaviour
{


    public int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth; // 👈 propiedad pública de solo lectura

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " recibió " + amount + " de daño. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió!");
        Destroy(gameObject);
    }
}
