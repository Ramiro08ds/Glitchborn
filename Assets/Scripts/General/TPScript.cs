using UnityEngine;

public class TPScript : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layerToTp;
    public Transform tpPos;
void OnTriggerEnter(Collider other)
{
    Debug.Log("Triggered TP");

    // ¿Coincide el layer?
    if (((1 << other.gameObject.layer) & layerToTp) != 0)
    {
        Debug.Log("TPING player");

        // Buscar el Player en los padres
        Transform root = other.transform.root;

        CharacterController controller = root.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        root.position = tpPos.position;

        if (controller != null) controller.enabled = true;
    }
}

}
