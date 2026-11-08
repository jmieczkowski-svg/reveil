using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Cel Kamery")]
    [Tooltip("Przeciągnij tutaj obiekt Player.")]
    public Transform target;

    [Tooltip("Wysokość, na którą patrzy kamera (np. klatka piersiowa/głowa).")]
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0);

    [Header("Ustawienia myszy i dystansu")]
    [Range(1f, 10f)] public float distance = 3.5f;
    [Range(0.5f, 5f)] public float mouseSensitivity = 2.0f;

    [Header("Ograniczenia osi Y (Pionowej)")]
    public float yMinLimit = -20f; // Jak bardzo w dół pod nogi można spojrzeć
    public float yMaxLimit = 60f;  // Jak bardzo w górę można spojrzeć

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        // Zablokowanie i ukrycie kursora myszy na środku ekranu
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Pobieranie ruchu myszki
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Blokowanie obrotu w pionie, żeby kamera nie przeszła przez sufit/podłogę
        pitch = Mathf.Clamp(pitch, yMinLimit, yMaxLimit);

        // Obliczanie rotacji i pozycji
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position + targetOffset;

        // Aplikowanie rotacji i pozycji do kamery
        transform.rotation = rotation;
        transform.position = position;
    }
}
