using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPScript : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layerToTp;
    public Transform tpPos;
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered TP");
        if (other.gameObject.layer == layerToTp)
        {
            Debug.Log("TPING player");
            other.transform.position = tpPos.position;
        } else
        {
            Debug.Log($"{other.gameObject.layer} was triggered");
        }
    }
}
