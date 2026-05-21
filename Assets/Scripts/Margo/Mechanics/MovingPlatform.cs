using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : ActivatableDevice // Наследуем от базы
{
    [Header("📍 Точки движения")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("⚙️ Настройки")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool startAtPointB = false;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        transform.position = (startAtPointB && pointB != null) ? pointB.position
                       : (pointA != null ? pointA.position : transform.position);
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving) return;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    // 🔑 ДОБАВЛЕНО override
    public override void Open() => Activate();
    public override void Close() => Deactivate();

    public void Activate()
    {
        if (pointB == null) return;
        targetPosition = pointB.position;
        isMoving = true;
    }

    public void Deactivate()
    {
        if (pointA == null) return;
        targetPosition = pointA.position;
        isMoving = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && !collision.rigidbody.isKinematic)
            collision.transform.SetParent(transform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.parent == transform)
            collision.transform.SetParent(null);
    }
}