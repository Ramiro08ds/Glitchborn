using UnityEngine;
using System.Collections;

public class IgrisHealth : MonoBehaviour
{
    public int maxHealth = 300;
    public int currentHealth;

    public bool isDead = false;
    public bool isStunned = false;

    private EnemyAnimatorController animatorController;
    private IgrisMovement movement;
    private IgrisAttack attack;

    // 🔥 NUEVO: feedback de daño
    public Renderer igrisRenderer;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    // Barra de vida
    public EnemyHealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        animatorController = GetComponent<EnemyAnimatorController>();
        movement = GetComponent<IgrisMovement>();
        attack = GetComponent<IgrisAttack>();

        if (healthBar == null)
            healthBar = GetComponentInChildren<EnemyHealthBar>();

        if (healthBar != null)
        {
            healthBar.target = transform;
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        // 🔥 Buscar renderer si no lo asignaste
        if (igrisRenderer == null)
            igrisRenderer = GetComponentInChildren<Renderer>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        // 🔥 Actualizar barra
        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        // 🔥 Efecto de daño
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Stun
        isStunned = true;
        movement.SetStunned(true);
        Invoke("RecoverFromStun", 1.2f);
    }

    IEnumerator DamageFlash()
    {
        if (igrisRenderer == null) yield break;

        Material mat = igrisRenderer.material;
        Color originalColor = mat.color;

        mat.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        mat.color = originalColor;
    }

    void RecoverFromStun()
    {
        if (isDead) return;
        isStunned = false;
        movement.SetStunned(false);
    }

    void Die()
    {
        isDead = true;

        animatorController.PlayDeath();
        movement.SetStunned(true);

        Destroy(gameObject, 4f);
    }
}
