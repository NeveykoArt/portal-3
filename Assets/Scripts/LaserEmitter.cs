using UnityEngine;
using System.Collections.Generic;

public class LaserEmitter : MonoBehaviour
{
    [Header("⚙️ Настройки луча")]
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private int maxReflections = 1;        // Сколько раз луч может отразиться
    [SerializeField] private float damageTime = 1.5f;       // Секунд контакта до смерти

    [Header("🔗 Компоненты")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform sourcePoint;         // Точка вылета луча
    [SerializeField] private LayerMask hitLayers;           // Слои, которые пересекает луч

    [Header(" Визуал")]
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color warningColor = Color.yellow;

    private LaserReceiver currentReceiver;
    private float contactTimer = 0f;

    private void Start()
    {
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
        if (sourcePoint == null) sourcePoint = transform;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
        UpdateLaserVisual(false);
    }

    private void Update()
    {
        List<Vector3> points = new List<Vector3> { sourcePoint.position };
        Vector3 currentPos = sourcePoint.position;
        Vector3 currentDir = sourcePoint.forward;
        bool isActive = false;
        float remainingRange = maxRange;

        // Сбрасываем предыдущий приёмник
        if (currentReceiver != null) currentReceiver.Deactivate();
        currentReceiver = null;

        // Просчитываем луч + отражения
        for (int i = 0; i <= maxReflections; i++)
        {
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, remainingRange, hitLayers))
            {
                points.Add(hit.point);
                remainingRange -= Vector3.Distance(currentPos, hit.point);

                //  Если попали в отражатель и есть лимит отражений
                if (hit.collider.TryGetComponent<LaserReflector>(out _) && i < maxReflections)
                {
                    currentDir = Vector3.Reflect(currentDir, hit.normal);
                    currentPos = hit.point + currentDir * 0.01f; // Микро-сдвиг, чтобы не зациклиться
                    continue;
                }

                // 🎯 Финальная точка (стена, приёмник, игрок)
                isActive = true;

                // Логика приёмника
                if (hit.collider.TryGetComponent(out LaserReceiver receiver))
                {
                    currentReceiver = receiver;
                    currentReceiver.Activate();
                }

                // Урон игроку
                if (hit.collider.CompareTag("Player"))
                {
                    contactTimer += Time.deltaTime;
                    lineRenderer.startColor = Color.Lerp(activeColor, warningColor, Mathf.Clamp01(contactTimer / damageTime));
                    if (contactTimer >= damageTime)
                    {
                        KillPlayer();
                        return;
                    }
                }
                else
                {
                    contactTimer = 0f;
                }
                break; // Луч остановился
            }
            else
            {
                // Не попали ни во что -> луч уходит в пустоту
                points.Add(currentPos + currentDir * remainingRange);
                isActive = false;
                break;
            }
        }

        // 📏 Обновляем LineRenderer под количество точек
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
            lineRenderer.SetPosition(i, points[i]);

        UpdateLaserVisual(isActive);
    }

    private void UpdateLaserVisual(bool isActive)
    {
        lineRenderer.startColor = isActive ? activeColor : inactiveColor;
        lineRenderer.endColor = Color.clear;
    }

    private void KillPlayer()
    {
        var menu = FindFirstObjectByType<MenuManager>();
        if (menu != null) menu.ShowLoseMenu();
        enabled = false;
        lineRenderer.enabled = false;
    }
}