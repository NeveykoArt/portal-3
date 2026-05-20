using UnityEngine;
using TMPro;

public class TutorialHintDisplay : MonoBehaviour
{
    public static TutorialHintDisplay Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (hintPanel != null)
            hintPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show(string text)
    {
        if (hintText != null) hintText.text = text;
        if (hintPanel != null) hintPanel.SetActive(true);
    }

    public void Hide()
    {
        if (hintPanel != null) hintPanel.SetActive(false);
    }
}
