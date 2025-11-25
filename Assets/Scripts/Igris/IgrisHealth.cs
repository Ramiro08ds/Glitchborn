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

    // Golpes consecutivos
    private int consecutiveHits = 0;
    public float hitResetTime = 1f;
    private float lastHitTime;

    // Feedback de daño
    public Renderer[] allRenderers;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

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

        // Obtener TODOS los renderers del boss (importante)
        allRenderers = GetComponentsInChildren<Renderer>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        // A veces el NavMeshAgent lo mueve y se buguea el frame
        StartCoroutine(ForceNavmeshSync());

        currentHealth -= dmg;

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        StartCoroutine(DamageFlash());

        // Reset de golpes
        if (Time.time - lastHitTime > hitResetTime)
            consecutiveHits = 0;

        consecutiveHits++;
        lastHitTime = Time.time;

        // Stun cada 2 golpes
        if (consecutiveHits >= 2)
        {
            consecutiveHits = 0;
            Stun();
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }
    }

    IEnumerator ForceNavmeshSync()
    {
        // Evita que el agente quede "fuera del overlapbox"
        yield return new WaitForEndOfFrame();

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(transform.position);
        }
    }

    void Stun()
    {
        isStunned = true;
        movement.SetStunned(true);
        Invoke("RecoverFromStun", 1.2f);
    }

    IEnumerator DamageFlash()
    {
        if (allRenderers == null || allRenderers.Length == 0)
            yield break;

        // Guardar materiales originales
        Color[] originalColors = new Color[allRenderers.Length];

        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i].material.HasProperty("_Color"))
                originalColors[i] = allRenderers[i].material.color;

            allRenderers[i].material.color = hitColor;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i].material.HasProperty("_Color"))
                allRenderers[i].material.color = originalColors[i];
        }
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
