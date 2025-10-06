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


        yield return new WaitForSeconds(delayBeforeHit);


        swordHitbox.Enable();


        yield return new WaitForSeconds(hitboxActiveTime);


        swordHitbox.Disable();


        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public int GetDamage()
    {
        int strength = playerLevel != null ? playerLevel.strength : 0;
        return 2 + strength * 1;
    }
}
