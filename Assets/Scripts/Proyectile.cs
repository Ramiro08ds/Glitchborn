using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager.instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy")) // evita que explote al tocar otro enemigo
        {
            Destroy(gameObject);
        }
    }
}
