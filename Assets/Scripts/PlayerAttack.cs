using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public float attackCooldown = 1.0f;
    public float delayBeforeHit = 0.12f;
    public float hitboxActiveTime = 0.3f;

    [Header("Referencias")]
    public SwordHitbox swordHitbox;

    private bool canAttack = true;

    void Start()
    {
        if (swordHitbox != null)
            swordHitbox.owner = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // Espera antes del impacto (para sincronizar con la animación)
        yield return new WaitForSeconds(delayBeforeHit);

        // Activa el hitbox
        swordHitbox.Enable();

        // Duración del golpe
        yield return new WaitForSeconds(hitboxActiveTime);

        // Desactiva el hitbox
        swordHitbox.Disable();

        // Cooldown antes del próximo ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public int GetDamage()
    {
        // 3 de daño base + 3 por cada punto de fuerza
        int strength = PlayerLevelSystem.Instance != null ? PlayerLevelSystem.Instance.strength : 1;
        return 3 + (strength - 1) * 3;
    }
}
