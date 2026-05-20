using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Elevator : MonoBehaviour
{
    public enum StartMode { OnSceneLoad, OnPlayerEnter, Manual }

    [Header("Движение")]
    [SerializeField] private Vector3 destinationOffset = new Vector3(0f, 10f, 0f);
    [SerializeField] private float speed = 2f;
    [SerializeField] private float startDelay = 1f;

    [Header("Запуск")]
    [SerializeField] private StartMode startMode = StartMode.OnPlayerEnter;
    [SerializeField] private string playerTag = "Player";

    [Header("События")]
    public UnityEvent onArrived;

    private Vector3 targetPosition;
    private bool moving;
    private bool finished;
    private Transform rider;
    private Transform riderOriginalParent;

    private void Start()
    {
        targetPosition = transform.position + destinationOffset;

        if (startMode == StartMode.OnSceneLoad)
            Invoke(nameof(BeginMove), startDelay);
    }

    public void BeginMove()
    {
        if (moving || finished) return;
        moving = true;
    }

    private void Update()
    {
        if (!moving) return;

        float step = speed * Time.deltaTime;
        Vector3 toTarget = targetPosition - transform.position;

        if (toTarget.sqrMagnitude <= step * step)
        {
            transform.position = targetPosition;
            moving = false;
            finished = true;
            DetachRider();
            onArrived?.Invoke();
            return;
        }

        transform.position += toTarget.normalized * step;
    }

    private void OnTriggerEnter(Collider other) => HandlePlayerEntry(other);

    private void OnCollisionEnter(Collision collision) => HandlePlayerEntry(collision.collider);

    private void HandlePlayerEntry(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag(playerTag)) return;

        AttachRider(other.transform);

        if (!moving && startMode == StartMode.OnPlayerEnter)
            Invoke(nameof(BeginMove), startDelay);
    }

    private void AttachRider(Transform t)
    {
        if (rider != null) return;
        rider = t;
        riderOriginalParent = t.parent;
        t.SetParent(transform, true);
    }

    private void DetachRider()
    {
        if (rider == null) return;
        rider.SetParent(riderOriginalParent, true);
        rider = null;
    }
}
