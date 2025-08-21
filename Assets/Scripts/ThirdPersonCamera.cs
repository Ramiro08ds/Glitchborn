using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // CameraTarget (empty en el pecho/cabeza del player)
    public Transform player; // referencia al Player (para rotarlo con la cámara)

    [Header("Configuración")]
    public float mouseSensitivity = 100f;
    public float distancia = 4f;
    public float altura = 2f;
    public float smoothSpeed = 10f;

    [Header("Límites verticales")]
    public float minY = -30f;
    public float maxY = 60f;

    private float rotacionX = 0f; // yaw (horizontal)
    private float rotacionY = 0f; // pitch (vertical)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || player == null) return;

        // Input del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Acumular rotaciones
        rotacionX += mouseX;
        rotacionY -= mouseY;

        // Limitar la rotación vertical
        rotacionY = Mathf.Clamp(rotacionY, minY, maxY);

        // Rotación de cámara
        Quaternion rotacion = Quaternion.Euler(rotacionY, rotacionX, 0);
        Vector3 direccion = rotacion * new Vector3(0, 0, -distancia);

        // Posición deseada de la cámara
        Vector3 posicionDeseada = target.position + Vector3.up * altura + direccion;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, smoothSpeed * Time.deltaTime);

        // La cámara mira al target
        transform.LookAt(target.position + Vector3.up * altura);

        // Rotar el player con la cámara (solo en Y)
        player.rotation = Quaternion.Euler(0, rotacionX, 0);
    }
}
