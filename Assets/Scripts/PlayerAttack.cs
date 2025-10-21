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
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // Espera antes de activar el golpe (sincronizar con animación)
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
        // Calcula el daño según fuerza o stats del jugador
        return 2; // o 2 + strength * 1 si tenías stats
    }
}
