using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Таймер")]
    [SerializeField] private bool useTimer = true;
    [SerializeField] private float timeLimit = 120f;

    [Header("UI (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Завершение уровня")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("События")]
    public UnityEvent onLevelComplete;

    private float timeRemaining;
    private bool levelCompleted = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        timeRemaining = timeLimit;
        UpdateTimerUI();
        StartCoroutine(FadeInRoutine());
    }

    private void Update()
    {
        if (!useTimer) return;
        if (timeRemaining <= 0f) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        UpdateTimerUI();
    }

    public void Win()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        onLevelComplete?.Invoke();
        StartCoroutine(FadeAndLoadRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        Image fadeImage = CreateFadeOverlay();
        yield return AnimateFade(fadeImage, 1f, 0f);
        Destroy(fadeImage.canvas.gameObject);
    }

    private IEnumerator FadeAndLoadRoutine()
    {
        Image fadeImage = CreateFadeOverlay();
        yield return AnimateFade(fadeImage, 0f, 1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            var sceneManager = FindFirstObjectByType<SceneManagerScript>();
            if (sceneManager != null)
                sceneManager.LoadSceneByName(nextSceneName);
            else
                Debug.LogWarning("SceneManagerScript не найден на сцене — переход не выполнен.");
        }
    }

    private IEnumerator AnimateFade(Image image, float fromAlpha, float toAlpha)
    {
        Color from = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fromAlpha);
        Color to   = new Color(fadeColor.r, fadeColor.g, fadeColor.b, toAlpha);

        image.color = from;

        float duration = Mathf.Max(0f, fadeDuration);
        if (duration <= 0f) { image.color = to; yield break; }

        yield return null;
        float startTime = Time.unscaledTime;

        while (true)
        {
            float elapsed = Time.unscaledTime - startTime;
            if (elapsed >= duration) break;
            image.color = Color.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        image.color = to;
    }

    private Image CreateFadeOverlay()
    {
        var canvasGo = new GameObject("LevelFadeCanvas");

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        canvasGo.AddComponent<CanvasScaler>();

        var imgGo = new GameObject("Fade");
        imgGo.transform.SetParent(canvasGo.transform, false);

        var image = imgGo.AddComponent<Image>();
        image.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        image.raycastTarget = false;

        var rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return image;
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        if (!useTimer)
        {
            if (timerText.gameObject.activeSelf)
                timerText.gameObject.SetActive(false);
            return;
        }

        if (!timerText.gameObject.activeSelf)
            timerText.gameObject.SetActive(true);

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }/*
    // 🔹 Вставь это в секцию [Header("События")] или рядом:
    [Header("Провал уровня")]
    [SerializeField] private UnityEvent onLevelFailed;
    private bool levelFailed = false;

    // 🔹 Вызываем проигрыш (из DeadlyGoo, лазеров и т.д.)
    public void Lose()
    {
        if (levelCompleted || levelFailed) return;
        levelFailed = true;
        useTimer = false; // Останавливаем таймер
        onLevelFailed?.Invoke(); // Позволяет команде подключить своё UI проигрыша
        StartCoroutine(FadeAndLoseRoutine());
    }

    // 🔹 Перезапуск уровня (для кнопки "Попробовать снова")
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var sceneManager = FindFirstObjectByType<SceneManagerScript>();
        if (sceneManager != null)
            sceneManager.LoadSceneByName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // 🔹 Рутина затемнения при проигрыше
    private IEnumerator FadeAndLoseRoutine()
    {
        Image fadeImage = CreateFadeOverlay();
        yield return AnimateFade(fadeImage, 0f, 1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // Полная пауза игры
    }*/
}
