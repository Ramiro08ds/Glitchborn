using UnityEngine;
using System.Collections;

public class IgrisHealth : MonoBehaviour
{
    [Header("Salud del Boss")]
    public int baseEnemyHealth = 100;
    public Animator animator;

    private int currentHealth;
    private bool isDead = false;
    private bool isStunned = false;
    private bool hasPlayedStun = false;

    private IgrisMovement movementScript;

    void Start()
    {
        currentHealth = baseEnemyHealth * 15; // 15x más vida que un enemigo normal
        movementScript = GetComponent<IgrisMovement>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning("⚠️ IgrisHealth: No se encontró Animator en hijos del objeto.");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // ✅ Stun una sola vez al llegar a 3/4 de vida
        if (!hasPlayedStun && currentHealth <= (baseEnemyHealth * 15 * 0.75f))
        {
            hasPlayedStun = true;
            StartCoroutine(TriggerStun());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator TriggerStun()
    {
        isStunned = true;

        if (animator != null)
            animator.SetBool("IsStunned", true);

        if (movementScript != null)
            movementScript.SetStunned(true);

        yield return new WaitForSeconds(4f); // duración del stun

        if (animator != null)
            animator.SetBool("IsStunned", false);

        if (movementScript != null)
            movementScript.SetStunned(false);

        isStunned = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (movementScript != null)
            movementScript.enabled = false;

        IgrisAttack attack = GetComponent<IgrisAttack>();
        if (attack != null)
            attack.enabled = false;

        StartCoroutine(DeathFade());
    }

    IEnumerator DeathFade()
    {
        yield return new WaitForSeconds(5f); // tiempo de animación de muerte

        Renderer[] rends = GetComponentsInChildren<Renderer>();
        float fadeDuration = 2f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            foreach (Renderer r in rends)
            {
                foreach (Material m in r.materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        Color c = m.color;
                        c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
                        m.color = c;
                    }
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    // ✅ Método público para otros scripts (EnemyAnimatorController)
    public bool IsStunned()
    {
        return isStunned;
    }
}
