using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwordHitbox : MonoBehaviour
{
    [HideInInspector] public PlayerAttack owner;

    [Header("Hitbox Settings")]
    [Tooltip("Tamaño local del box en unidades (xyz).")]
    public Vector3 boxSize = new Vector3(0.6f, 0.2f, 1f);
    [Tooltip("Offset local desde el transform del hitbox.")]
    public Vector3 localCenter = Vector3.zero;
    [Tooltip("Layer(s) que representan a los enemigos.")]
    public LayerMask enemyLayer;
    [Tooltip("Cada cuánto (segundos) comprobamos Overlap mientras el hitbox está activo. 0 = cada FixedUpdate.")]
    public float checkInterval = 0f; // 0 => FixedUpdate

    private Collider col;
    private HashSet<EnemyHealth> damagedThisSwing = new HashSet<EnemyHealth>();
    private bool isActive = false;
    private Coroutine activeRoutine;

    // Reuse array to avoid allocations (ajusta tamaño si hay muchos enemigos)
    private Collider[] overlapResults = new Collider[16];

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

    public void Enable()
    {
        damagedThisSwing.Clear();
        col.enabled = true;
        isActive = true;

        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ActiveCheckRoutine());
    }

    public void Disable()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = null;

        isActive = false;
        col.enabled = false;
    }

    IEnumerator ActiveCheckRoutine()
    {
        // Si checkInterval == 0 usamos FixedUpdate para sincronizar con physics
        if (checkInterval <= 0f)
        {
            while (isActive)
            {
                DoOverlapCheck();
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            var wait = new WaitForSeconds(checkInterval);
            while (isActive)
            {
                DoOverlapCheck();
                yield return wait;
            }
        }
    }

    void DoOverlapCheck()
    {
        // compute world center & halfExtents for OverlapBox
        Vector3 worldCenter = transform.TransformPoint(localCenter);
        Vector3 worldHalfExtents = Vector3.Scale(boxSize * 0.5f, transform.lossyScale);

        // Use OverlapBoxNonAlloc to avoid GC spikes
        int hits = Physics.OverlapBoxNonAlloc(worldCenter, worldHalfExtents, overlapResults, transform.rotation, enemyLayer);
        for (int i = 0; i < hits; i++)
        {
            var other = overlapResults[i];
            if (other == null) continue;
            HandleHitCollider(other);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // fallback en caso de que el Overlap no lo pillara
        HandleHitCollider(other);
    }

    void HandleHitCollider(Collider other)
    {
        if (owner == null) return;

        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null) return;

        if (damagedThisSwing.Contains(eh)) return;

        int damage = owner.GetDamage();
        eh.TakeDamage(damage);

        // Feedback
        if (HitFeedback.Instance != null)
        {
            HitFeedback.Instance.PlayHitFeedback(other.transform.position);
            HitFeedback.Instance.StartCoroutine(HitFeedback.Instance.HitStop(0.04f));
        }

        damagedThisSwing.Add(eh);
        Debug.Log($"SwordHitbox: golpeé a {eh.name} por {damage} de daño");
    }

    // Visualizador en editor para ajustar hitbox
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Vector3 worldCenter = transform.TransformPoint(localCenter);
        Vector3 worldSize = Vector3.Scale(boxSize, transform.lossyScale);
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, worldSize);
    }
}
