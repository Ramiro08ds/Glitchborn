using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HitFeedback : MonoBehaviour
{
    public static HitFeedback Instance;

    [Header("Cámara y sonido")]
    public Camera mainCamera;
    public AudioClip hitSound;
    public float shakeIntensity = 0.15f;
    public float shakeDuration = 0.1f;

    private AudioSource audioSource;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void PlayHitFeedback(Vector3 hitPoint)
    {
        if (mainCamera != null)
            StartCoroutine(Shake());
        if (audioSource && hitSound)
            audioSource.PlayOneShot(hitSound);
    }

    public IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    IEnumerator Shake()
    {
        Vector3 originalPos = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            mainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPos;
    }
}
