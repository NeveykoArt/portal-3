using UnityEngine;
using UnityEngine.InputSystem;

public class PortalGun : MonoBehaviour
{
    [SerializeField] private LayerMask portalableLayers;
    [SerializeField] private string portalableTag = "Portalable";
    [SerializeField] private GameObject bluePortalPrefab;
    [SerializeField] private GameObject orangePortalPrefab;
    
    [Header("Настройки снарядов")]
    [SerializeField] private GameObject blueProjectilePrefab;
    [SerializeField] private GameObject orangeProjectilePrefab;
    [SerializeField] private Transform shootPoint;
    
    private GameObject currentBluePortal;
    private GameObject currentOrangePortal;
    private InputSystem_Actions controls;
    private FirstPersonCamera playerCamera;

    void Awake() => controls = new InputSystem_Actions();

    void Start()
    {
        playerCamera = GetComponentInParent<FirstPersonCamera>();
        if (playerCamera == null)
            playerCamera = FindFirstObjectByType<FirstPersonCamera>();
    }
    
    void OnEnable()
    {
        controls.Enable();
        controls.Player.FireBlue.performed += ctx => ShootPortal(PortalColor.Blue);
        controls.Player.FireOrange.performed += ctx => ShootPortal(PortalColor.Orange);
    }
    
    void OnDisable() => controls.Disable();
    
    void ShootPortal(PortalColor color)
    {
        if (ToolModeManager.Instance != null && ToolModeManager.Instance.CurrentMode != ToolMode.Portal) return;//НОВОЕ

        if (playerCamera == null || shootPoint == null) return;

        // Направление на точку прицела
        Vector3 aimPoint = playerCamera.AimPoint;
        Vector3 shootDirection = (aimPoint - shootPoint.position).normalized;
        
        // Выбираем префаб снаряда
        GameObject projectilePrefab = (color == PortalColor.Blue) ? blueProjectilePrefab : orangeProjectilePrefab;
        GameObject portalPrefab = (color == PortalColor.Blue) ? bluePortalPrefab : orangePortalPrefab;
        
        if (projectilePrefab == null || portalPrefab == null) return;
        
        // Создаем снаряд
        GameObject projectileObj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        PortalProjectile projectile = projectileObj.GetComponent<PortalProjectile>();
        
        if (projectile != null)
        {
            projectile.portalColor = color;
            projectile.portalPrefab = portalPrefab;
            projectile.portalableLayers = portalableLayers;
            projectile.portalableTag = portalableTag;
            projectile.Initialize(shootDirection, this);
            projectileObj.transform.rotation = Quaternion.LookRotation(shootDirection);
        }
    }
    public void RemoveBluePortal()
    {
        if (currentBluePortal != null)
        {
            Destroy(currentBluePortal);
            currentBluePortal = null;
        }
    }
    
    public void RemoveOrangePortal()
    {
        if (currentOrangePortal != null)
        {
            Destroy(currentOrangePortal);
            currentOrangePortal = null;
        }
    }
    
    public void RegisterBluePortal(GameObject portal)
    {
        currentBluePortal = portal;
        CheckPortalLink();
    }
    
    public void RegisterOrangePortal(GameObject portal)
    {
        currentOrangePortal = portal;
        CheckPortalLink();
    }
    
    void CheckPortalLink()
    {
        if (currentBluePortal != null && currentOrangePortal != null)
        {
            Invoke(nameof(DelayedLinkCheck), 0.1f);
        }
    }
    
    void DelayedLinkCheck()
    {
        PortalController.Instance?.CheckAndLinkPortals();
    }
}