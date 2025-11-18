using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class SwordHitbox : MonoBehaviour
{
    [HideInInspector] public PlayerAttack owner;

    [Header("Hitbox Settings")]
    public Vector3 boxSize = new Vector3(0.6f, 0.2f, 1f);
    public Vector3 localCenter = Vector3.zero;
    public LayerMask enemyLayer;
    public float checkInterval = 0f;

    [Header("Knockback")]
    public float knockbackForce = 2f;

    private Collider col;
    // Cambié a HashSet<int> para permitir registrar cualquier enemigo (instanceID)
    private HashSet<int> damagedThisSwing = new HashSet<int>();
    private bool isActive = false;
    private Coroutine activeRoutine;
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

    public int GetHitCount()
    {
        return damagedThisSwing.Count;
    }

    IEnumerator ActiveCheckRoutine()
    {
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
        Vector3 worldCenter = transform.TransformPoint(localCenter);
        Vector3 worldHalfExtents = Vector3.Scale(boxSize * 0.5f, transform.lossyScale);

        int hits = Physics.OverlapBoxNonAlloc(worldCenter, worldHalfExtents, overlapResults, transform.rotation, enemyLayer);
        for (int i = 0; i < hits; i++)
        {
            var other = overlapResults[i];
            if (other != null) HandleHitCollider(other);
        }
    }

    void OnTriggerEnter(Collider other) => HandleHitCollider(other);

    void HandleHitCollider(Collider other)
    {
        if (owner == null) return;

        // Intentar obtener EnemyHealth
        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();

        // Si no hay EnemyHealth, intentar con IgrisHealth
        IgrisHealth ih = null;
        if (eh == null)
            ih = other.GetComponentInParent<IgrisHealth>();

        // Si no es ninguno de los dos, no es un objetivo válido
        if (eh == null && ih == null) return;

        // Identificador único para evitar doble golpe cada swing
        int uniqueId = (eh != null) ? eh.GetInstanceID() : ih.GetInstanceID();

        if (damagedThisSwing.Contains(uniqueId)) return;

        // Knockback seguro usando NavMeshAgent (se pasa el transform)
        Transform enemyTransform = (eh != null) ? eh.transform : ih.transform;
        ApplyKnockback(enemyTransform);

        // Daño según fuerza del jugador
        int damage = owner.GetDamage();
        if (eh != null) eh.TakeDamage(damage);
        if (ih != null) ih.TakeDamage(damage);

        // Reproducir sonido de golpe usando AudioManager (si existe)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SonidoPlayerGolpeaEnemigo();
        }

        // Feedback de daño del jugador (si existe)
        if (PlayerHitFeedback.instance != null)
            PlayerHitFeedback.instance.OnPlayerDamaged();

        damagedThisSwing.Add(uniqueId);
        Debug.Log($"Golpeado {(eh != null ? eh.name : ih.name)} por {damage} de daño");
    }

    void ApplyKnockback(Transform enemyTransform)
    {
        if (enemyTransform == null) return;

        NavMeshAgent agent = enemyTransform.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Vector3 dir = (enemyTransform.position - transform.position).normalized;
            Vector3 knockPos = enemyTransform.position + dir * knockbackForce;

            StartCoroutine(MoveEnemyNavMesh(agent, knockPos));
        }
    }

    IEnumerator MoveEnemyNavMesh(NavMeshAgent agent, Vector3 targetPos)
    {
        float elapsed = 0f;
        float duration = 0.15f;
        Vector3 start = agent.transform.position;

        while (elapsed < duration)
        {
            if (agent == null)
            {
                yield break;
            }

            Vector3 newPos = Vector3.Lerp(start, targetPos, elapsed / duration);
            // Mantengo la misma aproximación de desplazamiento lineal.
            // Uso agent.Warp al final para garantizar que el NavMeshAgent termine en la posición objetivo.
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Al final del movimiento intento ubicar al agente en la posición final de forma segura
        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(targetPos);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Vector3 worldCenter = transform.TransformPoint(localCenter);
        Vector3 worldSize = Vector3.Scale(boxSize, transform.lossyScale);
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, worldSize);
    }
}
