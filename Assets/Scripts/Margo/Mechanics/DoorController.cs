using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("⚙️ Настройки")]
    [SerializeField] private float moveSpeed = 4f;      // Скорость открытия/закрытия
    [SerializeField] private float openHeight = 5f;     // На сколько метров поднимается

    

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;
    private bool isOpen;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
        targetPosition = closedPosition;

        // Добавляем кинематичный Rigidbody, чтобы движущийся коллайдер не ломал физику
        if (!GetComponent<Rigidbody>())
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void Update()
    {
        // Плавное перемещение к целевой позиции
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            targetPosition = openPosition;
            
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            targetPosition = closedPosition;
            
        }
    }
}