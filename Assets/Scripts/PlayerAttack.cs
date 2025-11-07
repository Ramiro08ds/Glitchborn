using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración del Ataque")]
    public float attackCooldown = 0.5f;         // Tiempo mínimo entre ataques
    public float delayBeforeHit = 0.12f;        // Retraso antes del impacto (sincronizado con la animación)
    public float hitboxActiveTime = 0.3f;       // Tiempo que el hitbox permanece activo
    public float comboResetTime = 1.5f;         // Tiempo para reiniciar el combo si no se encadena el siguiente ataque

    [Header("Referencias")]
    public SwordHitbox swordHitbox;
    public Animator animator;                   // Asigna el Animator del Player

    private bool canAttack = true;
    private bool queuedNextAttack = false;      // Guarda si se hizo clic durante el ataque
    private int comboStep = 0;                  // 0 = sin combo, 1 = ataque1, 2 = ataque2
    private float lastAttackTime;

    void Start()
    {
        if (swordHitbox != null)
            swordHitbox.owner = this;
    }

    void Update()
    {
        // Reiniciar combo si pasa mucho tiempo sin atacar
        if (comboStep > 0 && Time.time - lastAttackTime > comboResetTime)
            comboStep = 0;

        // Si hace clic
        if (Input.GetMouseButtonDown(0))
        {
            // Si puede atacar, inicia el ataque normalmente
            if (canAttack)
            {
                if (comboStep == 0)
                {
                    comboStep = 1;
                    StartCoroutine(DoAttack("PjAtaque1"));
                }
                else if (comboStep == 1)
                {
                    comboStep = 2;
                    StartCoroutine(DoAttack("PjAtaque2"));
                }
            }
            // Si está en medio de un ataque, se marca que quiere encadenar el siguiente
            else
            {
                queuedNextAttack = true;
            }
        }
    }

    IEnumerator DoAttack(string animName)
    {
        canAttack = false;
        lastAttackTime = Time.time;
        queuedNextAttack = false;

        // Reproducir la animación correspondiente
        if (animator != null)
            animator.Play(animName);

        // Espera antes de activar el golpe
        yield return new WaitForSeconds(delayBeforeHit);

        // Activar hitbox
        swordHitbox.Enable();

        // Esperar mientras el hitbox está activo
        yield return new WaitForSeconds(hitboxActiveTime);

        // Desactivar hitbox
        swordHitbox.Disable();

        // Cooldown antes de permitir otro ataque
        yield return new WaitForSeconds(attackCooldown);

        // Si hay un clic guardado y todavía se puede encadenar → hace el siguiente ataque
        if (queuedNextAttack && comboStep == 1)
        {
            comboStep = 2;
            StartCoroutine(DoAttack("PjAtaque2"));
            yield break; // Termina este ataque y pasa al siguiente
        }

        // Si no hay clic guardado, reiniciar combo si ya fue el ataque2
        if (comboStep >= 2)
            comboStep = 0;

        canAttack = true;
    }

    // Daño del jugador (igual que antes)
    public int GetDamage()
    {
        int strength = PlayerLevelSystem.Instance != null ? PlayerLevelSystem.Instance.strength : 1;
        float damage = 3 + (strength - 1) * 1.2f;
        return Mathf.RoundToInt(damage);
    }
}

