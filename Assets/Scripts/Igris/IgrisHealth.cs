using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IgrisHealth : MonoBehaviour
{
    public int maxHealth = 300;
    public int currentHealth;
    public bool isDead = false;
    public bool isStunned = false;
    private EnemyAnimatorController animatorController;
    private IgrisMovement movement;
    private IgrisAttack attack;
    private int consecutiveHits = 0;
    public float hitResetTime = 1f;
    private float lastHitTime;
    public Renderer[] allRenderers;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    public EnemyHealthBar healthBar;
    public float stunDuration = 1.2f;

    [Header("=== RETURN TO MENU ===")]
    public float delayBeforeMainMenu = 4f;
    public string mainMenuSceneName = "MainMenu";

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

        allRenderers = GetComponentsInChildren<Renderer>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (healthBar != null)
            healthBar.UpdateHealthBar(currentHealth, maxHealth);

        StartCoroutine(DamageFlash());

        if (Time.time - lastHitTime > hitResetTime)
            consecutiveHits = 0;

        consecutiveHits++;
        lastHitTime = Time.time;

        Debug.Log("IGRIS HIT. Count = " + consecutiveHits);

        if (consecutiveHits >= 3)
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

    void Stun()
    {
        if (isDead) return;

        Debug.Log("IGRIS STUNNED!");
        isStunned = true;
        movement.SetStunned(true);

        if (attack != null)
            attack.enabled = false;

        Invoke(nameof(RecoverFromStun), stunDuration);
    }

    void RecoverFromStun()
    {
        if (isDead) return;

        Debug.Log("IGRIS RECOVERED");
        isStunned = false;
        movement.SetStunned(false);

        if (attack != null)
            attack.enabled = true;
    }

    void Die()
    {
        isDead = true;

        if (animatorController != null)
            animatorController.PlayDeath();

        movement.SetStunned(true);

        Debug.Log("¡Jefe derrotado! Volviendo al menú en " + delayBeforeMainMenu + " segundos...");

        // Iniciar coroutine que destruye Y carga escena
        StartCoroutine(DieSequence());
    }

    IEnumerator DieSequence()
    {
        // Esperar delay
        yield return new WaitForSeconds(delayBeforeMainMenu);

        Debug.Log("Cargando MainMenu...");

        // Cargar escena
        SceneManager.LoadScene(mainMenuSceneName);

        // Nota: No hace falta Destroy porque la escena se va a cambiar
    }

    IEnumerator DamageFlash()
    {
        yield break;
    }
}