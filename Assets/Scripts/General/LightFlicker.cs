using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light lightSource;
    public float minIntensity = 0.6f;
    public float maxIntensity = 1.2f;

    void Update()
    {
        lightSource.intensity = Random.Range(minIntensity, maxIntensity);
    }
}
