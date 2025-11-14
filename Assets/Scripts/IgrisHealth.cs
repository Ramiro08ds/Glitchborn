using UnityEngine;

public class IgrisHealth : MonoBehaviour
{
    public int maxHealth = 300;
    public int currentHealth;

    public bool isDead = false;
    public bool isStunned = false;

    private EnemyAnimatorController animatorController;
    private IgrisMovement movement;
    private IgrisAttack attack;

    void Start()
    {
        currentHealth = maxHealth;
        animatorController = GetComponent<EnemyAnimatorController>();
        movement = GetComponent<IgrisMovement>();
        attack = GetComponent<IgrisAttack>();
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Stun corto
        isStunned = true;
        movement.SetStunned(true);
        Invoke("RecoverFromStun", 1.2f);
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
