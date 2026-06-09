using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Co ma śledzić?")]
    public Transform target;
    
    [Header("Sztywne Ustawienia (Wpisuj tutaj)")]
    [Tooltip("Przesunięcie kamery względem gracza")]
    public Vector3 offset = new Vector3(0f, 15f, -10f);
    
    [Tooltip("Kąt patrzenia kamery (X to pochylenie w dół)")]
    public Vector3 rotationAngle = new Vector3(60f, 0f, 0f);

    [Header("Ustawienia Zooma")]
    public float smoothSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;  
    public float maxZoom = 20f; 

    private Camera cam;
    private float currentZoom = 15f; // Domyślna wartość startowa dla zooma

    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Startowy rozmiar dla trybu Orthographic
        if (cam.orthographic) 
        {
            currentZoom = cam.orthographicSize;
        }
        else 
        {
            // Startowy dystans dla trybu Perspective
            currentZoom = offset.magnitude; 
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. ZAWSZE wymuszamy sztywny kąt patrzenia (Kamera nigdy się nie obróci sama)
        transform.rotation = Quaternion.Euler(rotationAngle);

        // 2. Obsługa kółka myszy (Scroll)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            if (cam.orthographic)
            {
                cam.orthographicSize = currentZoom;
            }
        }

        // 3. Obliczanie docelowej pozycji
        Vector3 desiredPosition;
        if (cam.orthographic)
        {
            // W rzucie płaskim (Orthographic) offset jest stały, zoom zmienia się parametrem kamery
            desiredPosition = target.position + offset;
        }
        else
        {
            // W rzucie perspektywicznym zoom polega na przybliżaniu/oddalaniu kamery wzdłuż linii offsetu
            Vector3 zoomOffset = offset.normalized * currentZoom;
            desiredPosition = target.position + zoomOffset;
        }

        // 4. Płynne podążanie za graczem
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}