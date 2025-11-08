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
    private HashSet<EnemyHealth> damagedThisSwing = new HashSet<EnemyHealth>();
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

    // NUEVO: Método para obtener cuántos enemigos golpeó en este swing
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

        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null || damagedThisSwing.Contains(eh)) return;

        // Knockback seguro usando NavMeshAgent
        ApplyKnockback(eh);

        // Daño según fuerza del jugador
        int damage = owner.GetDamage();
        eh.TakeDamage(damage);


        // Feedback de daño del jugador
        if (PlayerHitFeedback.instance != null)
            PlayerHitFeedback.instance.OnPlayerDamaged();

        damagedThisSwing.Add(eh);
        Debug.Log($"Golpeado {eh.name} por {damage} de daño");
    }

    void ApplyKnockback(EnemyHealth enemy)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            Vector3 dir = (enemy.transform.position - transform.position).normalized;
            Vector3 knockPos = enemy.transform.position + dir * knockbackForce;

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
                Debug.Log("Hola");
                yield break;
            }

            Vector3 newPos = Vector3.Lerp(start, targetPos, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
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