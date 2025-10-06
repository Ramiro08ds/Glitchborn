using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwordHitbox : MonoBehaviour
{
    [HideInInspector] public PlayerAttack owner;
    private Collider col;
    private HashSet<EnemyHealth> damagedThisSwing = new HashSet<EnemyHealth>();

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        col.enabled = false;
    }

    // Llamar al comenzar la ventana de golpe
    public void Enable()
    {
        damagedThisSwing.Clear();
        col.enabled = true;
    }

    // Llamar al terminar la ventana de golpe
    public void Disable()
    {
        col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;

        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null) return;

        if (damagedThisSwing.Contains(eh)) return;

        int damage = owner.GetDamage(); // obtiene daño actual del PlayerAttack
        eh.TakeDamage(damage);
        damagedThisSwing.Add(eh);

        Debug.Log($"SwordHitbox: golpeé a {eh.name} por {damage} de daño");
    }
}
