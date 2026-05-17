using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    [Header("Настройки чувствительности")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private bool invertY = false;
    
    [Header("Ограничения углов")]
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;
    
    [Header("Настройки прицела")]
    [SerializeField] private float maxRange = 100f;
    [SerializeField] private LayerMask portalableLayers;
    [SerializeField] private string portalableTag = "Portalable";
    [SerializeField] private string reflectiveTag = "Reflective";
    
    // Публичное свойство для доступа из других скриптов
    public Vector3 AimPoint { get; private set; }
    public bool IsValidTarget { get; private set; }
    public bool IsReflectiveTarget { get; private set; } // Флаг для отражающей поверхности
    public Ray AimRay { get; private set; }
    public GameObject CurrentHitObject { get; private set; } // Объект, в который целится игрок
    
    private float xRotation = 0f;
    private float yRotation = 0f;
    private Transform playerBody;
    
    // Используем созданный класс PlayerControls
    private InputSystem_Actions playerControls;
    private Vector2 lookInput;
    
    void Awake()
    {
        playerControls = new InputSystem_Actions();
    }
    
    private void SetCameraToEyeLevel()
    {
        transform.localPosition = new Vector3(0f, 0.6f, 0f);
    }

    void Start()
    {
        playerBody = transform.parent;
        SetCameraToEyeLevel();
        LockCursor();
    }
    
    void OnEnable()
    {
        playerControls.Enable();
        
        // Подписываемся на ввод
        playerControls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        playerControls.UI.Cancel.performed += ctx => UnlockCursor();
    }
    
    void OnDisable()
    {
        playerControls.Disable();
    }
    
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                LockCursor();
            }
            return;
        }
        
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        
        if (invertY)
            mouseY = -mouseY;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        yRotation += mouseX;
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        UpdateAim();
    }
    
    void UpdateAim()
    {
        // Создаем луч из центра экрана
        AimRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        // Проверяем попадание
        if (Physics.Raycast(AimRay, out RaycastHit hit, maxRange, portalableLayers))
        {
            AimPoint = hit.point;

            if (hit.collider.CompareTag(reflectiveTag))
            {
                IsReflectiveTarget = true;
                IsValidTarget = false;
            }
            else if (hit.collider.CompareTag(portalableTag))
            {
                IsValidTarget = true;
            }
            else
            {
                IsValidTarget = false;
            }
        }
        else
        {
            // Если ничего не попали, ставим точку далеко по направлению луча
            AimPoint = AimRay.origin + AimRay.direction * maxRange;
            IsValidTarget = false;
            IsReflectiveTarget = false;
        }
    }
    
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        playerControls.Player.Enable();
        playerControls.UI.Disable();
    }
    
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        playerControls.Player.Disable();
        playerControls.UI.Enable();
    }
    
    private void OnApplicationFocus(bool focus)
    {
        if (focus && Cursor.lockState == CursorLockMode.Locked)
        {
            LockCursor();
        }
    }
}