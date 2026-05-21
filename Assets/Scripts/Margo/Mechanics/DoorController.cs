using UnityEngine;

public class DoorController : ActivatableDevice // Наследуем от базы
{
    [Header("⚙️ Настройки")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float openHeight = 5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;
    private bool isOpen;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
        targetPosition = closedPosition;

        if (!GetComponent<Rigidbody>())
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // 🔑 ДОБАВЛЕНО override
    public override void Open()
    {
        if (!isOpen)
        {
            isOpen = true;
            targetPosition = openPosition;
        }
    }

    public override void Close()
    {
        if (isOpen)
        {
            isOpen = false;
            targetPosition = closedPosition;
        }
    }
}