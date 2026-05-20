using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuUI;     // панель с UI паузы (должна быть отключена по умолчанию)
    [SerializeField] private SceneManagerScript sceneManager; // ссылка на ваш скрипт управления сценой

    private AudioSource audioSource;

    private InputAction pauseAction;
    private bool isPaused = false;

    private void Awake()
    {
        pauseAction = new InputAction(binding: "<Keyboard>/escape");

        if (sceneManager == null)
            sceneManager = FindFirstObjectByType<SceneManagerScript>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        
        sceneManager?.frezeScene();
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        sceneManager?.unfrezeScene();
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}