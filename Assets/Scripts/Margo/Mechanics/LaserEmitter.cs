using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LaserEmitter : MonoBehaviour
{
    [Header("⚙️ Настройки луча")]
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private int maxReflections = 1;
    [SerializeField] private float damageTime = 1.5f;

    [Header("🔗 Компоненты")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform sourcePoint;
    [SerializeField] private LayerMask hitLayers;

    [Header("🎨 Визуал")]
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color warningColor = Color.yellow;

    private LaserReceiver currentReceiver;
    private float contactTimer = 0f;
    private bool hasKilled = false;

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

        if (currentReceiver != null) currentReceiver.Deactivate();
        currentReceiver = null;

        for (int i = 0; i <= maxReflections; i++)
        {
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, remainingRange, hitLayers))
            {
                points.Add(hit.point);
                remainingRange -= Vector3.Distance(currentPos, hit.point);

                if (hit.collider.TryGetComponent<LaserReflector>(out _) && i < maxReflections)
                {
                    currentDir = Vector3.Reflect(currentDir, hit.normal);
                    currentPos = hit.point + currentDir * 0.01f;
                    continue;
                }

                isActive = true;

                if (hit.collider.TryGetComponent(out LaserReceiver receiver))
                {
                    currentReceiver = receiver;
                    currentReceiver.Activate();
                }

                if (hit.collider.CompareTag("Player") && !hasKilled)
                {
                    contactTimer += Time.deltaTime;
                    lineRenderer.startColor = Color.Lerp(activeColor, warningColor, Mathf.Clamp01(contactTimer / damageTime));
                    if (contactTimer >= damageTime)
                    {
                        hasKilled = true;
                        ReloadScene();
                        return;
                    }
                }
                else if (!hit.collider.CompareTag("Player"))
                {
                    contactTimer = 0f;
                }
                break;
            }
            else
            {
                points.Add(currentPos + currentDir * remainingRange);
                isActive = false;
                break;
            }
        }

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

    private void ReloadScene()
    {
        var sceneManager = FindFirstObjectByType<SceneManagerScript>();
        if (sceneManager != null)
        {
            sceneManager.LoadSceneByName(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}