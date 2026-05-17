using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Ограничения")]
    [SerializeField] private int maxPortalShots = 10;
    [SerializeField] private float timeLimit = 120f;

    [Header("UI (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI portalsLeftText;
    [SerializeField] private TextMeshProUGUI timerText;

    private int remainingPortals;
    private float timeRemaining;
    private bool isGameActive = false;
    private bool isGameFinished = false;

    private MenuManager menuManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        menuManager = FindFirstObjectByType<MenuManager>();
        ResetGameState();
        UpdateUI();
    }

    private void Update()
    {
        if (isGameActive && !isGameFinished)
        {
            // Уменьшаем время только если оно положительное
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                
                // Если стало меньше или равно 0 - проигрыш
                if (timeRemaining <= 0f)
                {
                    timeRemaining = 0f;
                    UpdateTimerUI();     // обновляем UI с нулём
                    Lose();
                    return;              // сразу выходим, чтобы не обновлять лишний раз
                }
            }
            
            UpdateTimerUI();
        }
    }

    private void ResetGameState()
    {
        remainingPortals = maxPortalShots;
        timeRemaining = timeLimit;
        isGameActive = false;
        isGameFinished = false;
        UpdateUI();
    }

    public void StartGame()
    {
        ResetGameState();
        isGameActive = true;
        isGameFinished = false;
        UpdateUI();
    }

    public bool TryUsePortal()
    {
        if (!isGameActive || isGameFinished)
            return false;

        if (remainingPortals <= 0)
            return false;

        remainingPortals--;
        UpdatePortalsUI();
        return true;
    }

    public void Win()
    {
        if (!isGameActive || isGameFinished)
            return;

        isGameActive = false;
        isGameFinished = true;

        if (menuManager != null)
            menuManager.OnPlayerWin(GameObject.FindGameObjectWithTag("Player"));
        else
            Debug.LogWarning("MenuManager not found!");
    }

    private void Lose()
    {
        if (!isGameActive || isGameFinished)
            return;

        isGameActive = false;
        isGameFinished = true;

        // Показываем меню проигрыша
        if (menuManager != null)
        {
            menuManager.ShowLoseMenu();
        }
        else
        {
            Debug.LogWarning("MenuManager not found! Restarting...");
            Invoke(nameof(RestartLevel), 2f);
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateUI()
    {
        UpdatePortalsUI();
        UpdateTimerUI();
    }

    private void UpdatePortalsUI()
    {
        if (portalsLeftText != null)
            portalsLeftText.text = $"{remainingPortals}";
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void RefillPortals(int amount)
    {
        if (!isGameActive || isGameFinished) return;
        
        remainingPortals += amount;
        // Не даём превысить максимальный лимит
        if (remainingPortals > maxPortalShots)
            remainingPortals = maxPortalShots;
        
        UpdatePortalsUI();
        
        // По желанию: звук, визуальный эффект, сообщение в консоль
        Debug.Log($"Portals refilled! Now: {remainingPortals}/{maxPortalShots}");
    }
}