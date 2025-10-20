using UnityEngine;
using System.Collections;

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

    [Header("Feedback")]
    public Renderer enemyRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public float knockbackForce = 3f;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar == null)
            healthBar = GetComponentInChildren<EnemyHealthBar>();

        if (enemyRenderer == null)
            enemyRenderer = GetComponentInChildren<Renderer>();

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

        // Efectos visuales y físicos
        StartCoroutine(FlashDamage());
        StartCoroutine(ApplyKnockback());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashDamage()
    {
        if (enemyRenderer == null) yield break;
        Material mat = enemyRenderer.material;
        Color original = mat.color;
        mat.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        mat.color = original;
    }

    IEnumerator ApplyKnockback()
    {
        if (Camera.main == null) yield break;
        Vector3 dir = (transform.position - Camera.main.transform.position).normalized;
        transform.position += dir * knockbackForce * Time.deltaTime;
        yield return null;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió!");

        if (player != null)
            player.EnemyKilled(xpReward, healOnKill);

        Destroy(gameObject);
    }
}
