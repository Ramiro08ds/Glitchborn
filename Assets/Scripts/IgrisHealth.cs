using UnityEngine;
using System.Collections;

public class IgrisHealth : MonoBehaviour
{
    public int baseEnemyHealth = 100;
    private int currentHealth;
    private bool isDead = false;
    private bool isStunned = false;
    private bool hasPlayedStun = false;

    private IgrisMovement movementScript;
    private EnemyAnimatorController animatorController;

    void Start()
    {
        currentHealth = baseEnemyHealth * 15;
        movementScript = GetComponent<IgrisMovement>();
        animatorController = GetComponent<EnemyAnimatorController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (!hasPlayedStun && currentHealth <= baseEnemyHealth * 15 * 0.75f)
        {
            hasPlayedStun = true;
            StartCoroutine(TriggerStun());
        }

        if (currentHealth <= 0) Die();
    }

    IEnumerator TriggerStun()
    {
        isStunned = true;
        movementScript.SetStunned(true);
        yield return new WaitForSeconds(4f);
        movementScript.SetStunned(false);
        isStunned = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        movementScript.SetStunned(true);
        animatorController.PlayDeath();

        StartCoroutine(DeathFade());
    }

    IEnumerator DeathFade()
    {
        yield return new WaitForSeconds(5f); // esperar animación de muerte

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

    public bool IsStunned() => isStunned;
}
