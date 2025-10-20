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
    public PlayerLevelUI menu;
    private PlayerLevelSystem playerLevel;
    private bool canAttack = true;

    void Start()
    {
        playerLevel = GetComponent<PlayerLevelSystem>();
        if (swordHitbox != null)
            swordHitbox.owner = this;
    }

    void Update()
    {
        if (menu != null && menu.menuAbierto)
            return;

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
        int strength = playerLevel != null ? playerLevel.strength : 0;
        return 2 + strength * 1;
    }
}
