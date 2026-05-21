using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("🔗 Подключение")]
    [SerializeField] private ActivatableDevice connectedDevice; // Было: IActivatable или DoorController
    [Header("⚙️ Настройки кнопки")]
    [SerializeField] private float pressedHeight = 0.8f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float activationThreshold = 0.2f;

    [Header("📊 Состояние")]
    [SerializeField] private float currentPressAmount = 0f;
    [SerializeField] private bool isActivated = false;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private Rigidbody rb;
    private float totalWeight = 0f;
    private HashSet<Collider> objectsOnPlate = new HashSet<Collider>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Убеждаемся, что коллайдер НЕ триггер
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;
    }

    private void Start()
    {
        originalPosition = transform.position;
        pressedPosition = originalPosition - Vector3.up * pressedHeight;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Просто проверяем, что объект сверху по Y
        if (collision.transform.position.y >= transform.position.y - 0.1f)
        {
            AddWeight(collision.collider, collision.rigidbody);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Проверяем каждый кадр, что объект всё ещё сверху
        if (collision.transform.position.y >= transform.position.y - 0.1f)
        {
            AddWeight(collision.collider, collision.rigidbody);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        RemoveWeight(collision.collider);
    }

    private void AddWeight(Collider col, Rigidbody rb)
    {
        if (objectsOnPlate.Contains(col)) return;

        float weight = 0f;

        // Проверяем массу
        if (rb != null)
        {
            weight = rb.mass;
        }
        else if (col.CompareTag("Player"))
        {
            weight = 1f; // Примерная масса игрока
        }
        else if (col.TryGetComponent<MagnetScript>(out _))
        {
            weight = 1f; // Груз
        }

        if (weight > 0f)
        {
            objectsOnPlate.Add(col);
            totalWeight += weight;
            Debug.Log($"[PressurePlate] ADD: {col.name}, Weight: {weight}, Total: {totalWeight}");
            UpdatePressAmount();
        }
    }

    private void RemoveWeight(Collider col)
    {
        if (!objectsOnPlate.Contains(col)) return;

        float weight = 0f;
        Rigidbody rb = col.attachedRigidbody;

        if (rb != null)
        {
            weight = rb.mass;
        }
        else if (col.CompareTag("Player"))
        {
            weight = 1f;
        }
        else if (col.TryGetComponent<MagnetScript>(out _))
        {
            weight = 1f;
        }

        objectsOnPlate.Remove(col);
        totalWeight -= weight;
        if (totalWeight < 0f) totalWeight = 0f;

        Debug.Log($"[PressurePlate] REMOVE: {col.name}, Weight: {weight}, Total: {totalWeight}");
        UpdatePressAmount();
    }

    private void UpdatePressAmount()
    {
        // Нормализуем вес (максимум 5 единиц для полного нажатия)
        currentPressAmount = Mathf.Clamp01(totalWeight / 5f);

        bool shouldBeActivated = currentPressAmount >= activationThreshold;

        if (shouldBeActivated && !isActivated)
        {
            isActivated = true;
            Debug.Log($"[PressurePlate] ACTIVATED!");
            connectedDevice?.Open();
        }
        else if (!shouldBeActivated && isActivated)
        {
            isActivated = false;
            Debug.Log($"[PressurePlate] DEACTIVATED!");
            connectedDevice?.Close();
        }
    }

    private void Update()
    {
        // Плавное движение кнопки
        float targetY = Mathf.Lerp(originalPosition.y, pressedPosition.y, currentPressAmount);
        Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * returnSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isActivated ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}