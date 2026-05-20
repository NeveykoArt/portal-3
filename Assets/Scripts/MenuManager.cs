using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("Меню")]
    [SerializeField] private GameObject startMenu; // Меню старта игры
    [SerializeField] private GameObject winMenu;   // Меню победы
    [SerializeField] private GameObject loseMenu;  // ← новое меню проигрыша
    
    [Header("Кнопки")]
    [SerializeField] private Button startButton;   // Кнопка "Начать"
    [SerializeField] private Button playAgainButton; // Кнопка "Играть еще"
    [SerializeField] private Button tryAgainButton;  // ← кнопка в меню проигрыша

    private bool pauseGameOnWin = true; // Останавливать ли игру при победе
    
    private GameObject playerObject;
    private FirstPersonCamera playerCameraScript;
    private PlayerInput playerInput;
    private bool isGameCompleted = false;
    
    private LevelManager levelManager;

    private void Start()
    {
        // Находим игрока и его компоненты
        FindPlayer();
        
        // Принудительно отключаем управление игроком с самого начала
        DisablePlayerControl();
        
        // Показываем меню старта
        ShowStartMenu();
        
        // Назначаем обработчики для кнопок
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
            
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(RestartGame);

        // В Start() добавить:
        if (tryAgainButton != null)
            tryAgainButton.onClick.AddListener(RestartGame);

        levelManager = LevelManager.Instance;
    }
    
    private void FindPlayer()
    {
        if (playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");
            
        if (playerObject != null)
        {
            // Ищем скрипт камеры на самом игроке или его детях
            playerCameraScript = playerObject.GetComponentInChildren<FirstPersonCamera>();
            
            // Ищем компонент PlayerInput (для InputSystem)
            playerInput = playerObject.GetComponent<PlayerInput>();
        }
    }
    
    // Показать меню старта
    public void ShowStartMenu()
    {
        if (startMenu != null)
        {
            startMenu.SetActive(true);
            // Ставим игру на паузу, пока в меню
            Time.timeScale = 0f;
            
            // Разблокируем курсор для меню
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        if (winMenu != null)
            winMenu.SetActive(false);
            
        isGameCompleted = false;
    }
    
    // Показать меню победы
    public void ShowWinMenu()
    {
        if (winMenu != null)
        {
            winMenu.SetActive(true);
        }
        
        if (startMenu != null)
            startMenu.SetActive(false);
            
        isGameCompleted = true;
        
        // Останавливаем игру
        if (pauseGameOnWin)
            Time.timeScale = 0f;
            
        // Разблокируем курсор для меню
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
            
        // Отключаем управление игроком
        DisablePlayerControl();
    }
    
    public void ShowLoseMenu()
    {
        if (loseMenu != null)
            loseMenu.SetActive(true);
        
        if (winMenu != null)
            winMenu.SetActive(false);
        if (startMenu != null)
            startMenu.SetActive(false);
        
        isGameCompleted = true;
        
        // Останавливаем игру
        Time.timeScale = 0f;
        
        // Отключаем управление
        DisablePlayerControl();
        
        // Разблокируем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Скрыть все меню
    public void HideAllMenus()
    {
        if (startMenu != null)
            startMenu.SetActive(false);
        if (winMenu != null)
            winMenu.SetActive(false);
        if (loseMenu != null)
            loseMenu.SetActive(false);
        
        Time.timeScale = 1f;
    }
    
    // Начать игру (кнопка "Начать")
    public void StartGame()
    {
        HideAllMenus();
        
        // ВКЛЮЧАЕМ управление игроком - теперь он может двигаться!
        EnablePlayerControl();

        if (levelManager != null)
            levelManager.StartGame();
        if (ToolModeManager.Instance != null) ToolModeManager.Instance.SetMode(ToolMode.Magnet);//НОВОЕ
    }
    
    // Играть еще (кнопка в меню победы)
    public void RestartGame()
    {
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Метод для вызова при победе (например, из триггера)
    public void OnPlayerWin(GameObject player)
    {
        if (!isGameCompleted)
        {
            playerObject = player;
            ShowWinMenu();
        }
    }
    
    // Отключение управления игроком
    private void DisablePlayerControl()
    {
        FindPlayer(); // Обновляем ссылку на игрока
            
        if (playerObject != null)
        {
            // Отключаем скрипт камеры (FirstPersonCamera)
            if (playerCameraScript != null)
            {
                playerCameraScript.enabled = false;
            }
            
            // Отключаем PlayerInput (важно для InputSystem)
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            // Разблокируем курсор (чтобы не было проблем)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (ToolModeManager.Instance != null) ToolModeManager.Instance.SetMode(ToolMode.Magnet);//Новое
        }
    }
    
    // Включение управления игроком
    private void EnablePlayerControl()
    {
        FindPlayer(); // Обновляем ссылку на игрока
            
        if (playerObject != null)
        {
            
            // Включаем PlayerInput (важно для InputSystem)
            if (playerInput != null)
            {
                playerInput.enabled = true;
            }
            
            // Включаем скрипт камеры (FirstPersonCamera)
            if (playerCameraScript != null)
            {
                playerCameraScript.enabled = true;
            }
            
            // Блокируем курсор для игры
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}