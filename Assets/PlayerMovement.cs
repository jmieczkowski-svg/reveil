using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ustawienia Gracza")]
    public float moveSpeed = 7f;

    private Rigidbody rb;
    private Camera mainCam;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1. Odbiór danych z klawiatury (WASD lub strzałki)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");
        movement = movement.normalized; // Zapobiega szybszemu poruszaniu się po skosie
    }

    void FixedUpdate()
    {
        // 2. Aplikowanie fizycznego ruchu
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // 3. Śledzenie kursora myszy na podłodze
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Wirtualna płaszczyzna na poziomie podłogi
        float rayLength;

        if (groundPlane.Raycast(ray, out rayLength))
        {
            Vector3 pointToLook = ray.GetPoint(rayLength);
            
            // Obliczenie kierunku (ignorujemy oś Y, by postać nie patrzyła w ziemię)
            Vector3 direction = new Vector3(pointToLook.x - transform.position.x, 0f, pointToLook.z - transform.position.z);

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(targetRotation);
            }
        }
    }
}