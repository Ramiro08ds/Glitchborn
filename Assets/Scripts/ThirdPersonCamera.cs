using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public PlayerMovement playerMovement; // Referencia al script del player
    public float distance = 3f;
    public float height = 1.5f;
    public float rotationSmoothTime = 0.1f;

    float pitch;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    void LateUpdate()
    {
        float mouseY = Input.GetAxis("Mouse Y") * playerMovement.mouseSensitivity * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        Vector3 targetRotation = new Vector3(pitch, player.eulerAngles.y);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        transform.position = player.position - transform.forward * distance + Vector3.up * height;
        transform.eulerAngles = currentRotation;
    }
}