using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ModernPlayerController : MonoBehaviour
{
    [Header("Referencja do Kamery")]
    [Tooltip("Przeciągnij tutaj Main Camera z okna Hierarchy")]
    public Transform cameraTransform;

    [Header("Parametry Ruchu")]
    [Tooltip("Szybkość chodu rycerza. Zostawiamy niską dla klimatu horroru.")]
    [Range(1f, 8f)] public float walkSpeed = 3.0f;
    [Tooltip("Jak szybko model postaci obraca się w kierunku ruchu.")]
    [Range(5f, 20f)] public float rotationSpeed = 10f;

    [Header("Grawitacja i Skok")]
    [Tooltip("Wysokość skoku. Rycerz w zbroi powinien skakać nisko i ociężale.")]
    [Range(0f, 3f)] public float jumpHeight = 1.0f;
    public float gravity = -9.81f;

    [Tooltip("Bufor skoku w sekundach. Eliminuje uczucie opóźnienia lub braku reakcji.")]
    public float jumpBufferTime = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float jumpBufferCounter;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Ruch względem kamery
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * vertical + right * horizontal;

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            controller.Move(moveDirection.normalized * walkSpeed * Time.deltaTime);
        }

        // 2. Buforowanie Skoku (Zapisujemy intencję gracza)
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 3. Grawitacja i wywołanie skoku
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -2f; // Lekki docisk do ziemi
            }

            // Jeśli gracz chciał skoczyć (bufor jest aktywny)
            if (jumpBufferCounter > 0f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpBufferCounter = 0f; // Resetujemy bufor po udanym skoku
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}