using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // arrastrá acá tu CameraTarget (empty en el pecho/cabeza del player)

    [Header("Configuración")]
    public float mouseSensitivity = 100f; // sensibilidad del mouse
    public float distancia = 4f;          // qué tan lejos de la cámara al jugador
    public float altura = 2f;             // qué tan alta la cámara respecto al jugador
    public float smoothSpeed = 10f;       // suavizado del movimiento

    [Header("Límites verticales")]
    public float minY = -30f;  // límite inferior (mirar hacia abajo)
    public float maxY = 60f;   // límite superior (mirar hacia arriba)

    private float rotacionX = 0f; // yaw (horizontal)
    private float rotacionY = 0f; // pitch (vertical)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // el cursor se queda en el centro
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Input del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Acumular rotaciones
        rotacionX += mouseX;
        rotacionY -= mouseY;

        // Limitar la rotación vertical
        rotacionY = Mathf.Clamp(rotacionY, minY, maxY);

        // Calcular la rotación de la cámara
        Quaternion rotacion = Quaternion.Euler(rotacionY, rotacionX, 0);

        // Posición de la cámara con offset
        Vector3 direccion = rotacion * new Vector3(0, altura, -distancia);

        // Aplicar la posición suavizada
        Vector3 posicionDeseada = target.position + direccion;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, smoothSpeed * Time.deltaTime);

        // Que la cámara siempre mire al target
        transform.LookAt(target.position + Vector3.up * altura);
    }
}
