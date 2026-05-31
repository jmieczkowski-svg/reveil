using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HybridMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    
    private NavMeshAgent agent;
    private Camera mainCam;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCam = Camera.main;
        
        // Konfiguracja agenta z poziomu kodu
        agent.speed = moveSpeed;
        agent.angularSpeed = 720f; // Bardzo szybkie i płynne obracanie się postaci
    }

    void Update()
    {
        HandleMouseClick();
        HandleWASD();
    }

    void HandleMouseClick()
    {
        // Wykrywa kliknięcie Lewym Przyciskiem Myszy
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Odblokowuje agenta i wysyła go w kliknięte miejsce
                agent.isStopped = false;
                agent.SetDestination(hit.point);
            }
        }
    }

    void HandleWASD()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

        // Jeśli gracz wciśnie jakikolwiek klawisz ruchu
        if (movement.magnitude >= 0.1f)
        {
            // Zatrzymuje automatyczny ruch z myszki
            agent.isStopped = true;

            // Przesuwa postać manualnie
            Vector3 moveDirection = movement * moveSpeed * Time.deltaTime;
            agent.Move(moveDirection);

            // Obraca postać w kierunku, w którym idzie
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }
}