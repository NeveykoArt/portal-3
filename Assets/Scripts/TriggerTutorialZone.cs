using UnityEngine;
using UnityEngine.UI;

public class TriggerTutorialZone : MonoBehaviour
{
    [Header("🖼️ UI Элементы")]
    [SerializeField] private GameObject tutorialPanel; // Ссылка на Panel в Canvas
    [SerializeField] private Text tutorialText;        // Ссылка на Text внутри панели

    [Header("📝 Контент")]
    [TextArea(3, 5)]
    [SerializeField] private string message = "ЛКМ — притянуть куб\nПКМ — толкнуть куб";

    [Header("⚙️ Логика")]
    [Tooltip("Закрыть панель по клику мыши или любой клавише")]
    [SerializeField] private bool closeOnInput = true;

    [Tooltip("0 = отключено. >0 = автоматически скрыть через N секунд")]
    [SerializeField] private float autoCloseTime = 0f;

    private bool hasShown = false;

    private void Start()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (tutorialText != null) tutorialText.text = message;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег игрока и флаг "уже показывали"
        if (other.CompareTag("Player") && !hasShown)
        {
            ShowPanel();
        }
    }

    private void ShowPanel()
    {
        hasShown = true;
        tutorialPanel?.SetActive(true);

        if (autoCloseTime > 0)
            Invoke(nameof(HidePanel), autoCloseTime);
    }

    public void HidePanel()
    {
        tutorialPanel?.SetActive(false);
        CancelInvoke(nameof(HidePanel));
    }

    private void Update()
    {
        if (hasShown && tutorialPanel != null && tutorialPanel.activeSelf && closeOnInput)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKeyDown)
                HidePanel();
        }
    }
}