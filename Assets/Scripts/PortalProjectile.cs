using UnityEngine;

public class PortalProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float portalThickness = 0.05f;
    
    public LayerMask portalableLayers;
    public LayerMask reflectiveLayers;
    
    public PortalColor portalColor;
    public GameObject portalPrefab;
    public string portalableTag = "Portalable";
    public string reflectiveTag = "Reflective";
    
    private PortalGun ownerGun;
    
    private Vector3 direction;
    private bool hasHit = false;
    
    void Start()
    {
        Destroy(gameObject, 5f);
    }
    
    public void Initialize(Vector3 shootDirection, PortalGun gun)
    {
        direction = shootDirection.normalized;
        transform.forward = direction;
        ownerGun = gun;
    }
    
    void Update()
    {
        if (hasHit) return;
        
        float moveDistance = speed * Time.deltaTime;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, moveDistance, portalableLayers | reflectiveLayers))
        {
            transform.position = hit.point;
            ProcessHit(hit);
        }
        else
        {
            transform.position += direction * moveDistance;
        }
    }
    
    void ProcessHit(RaycastHit hit)
    {
        hasHit = true;
        
        string hitTag = hit.collider.tag;
        
        if (hitTag == portalableTag)
        {
            CreatePortal(hit);
            Destroy(gameObject);
        }
        else if (hitTag == reflectiveTag)
        {
            Vector3 newDirection = Vector3.Reflect(direction, hit.normal);
            direction = newDirection;
            transform.forward = direction;
            transform.position = hit.point + direction * 0.1f;
            hasHit = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void CreatePortal(RaycastHit hit)
    {
        if (portalPrefab == null || ownerGun == null) return;
        
        // Удаляем старый портал через пушку
        if (portalColor == PortalColor.Blue)
        {
            ownerGun.RemoveBluePortal();
        }
        else
        {
            ownerGun.RemoveOrangePortal();
        }
        
        // Создаем новый портал
        Quaternion rotation = CalculatePortalRotation(hit.normal);
        Vector3 position = hit.point + hit.normal * portalThickness;
        
        GameObject newPortal = Instantiate(portalPrefab, position, rotation);
        
        // Регистрируем портал в пушке
        if (portalColor == PortalColor.Blue)
        {
            ownerGun.RegisterBluePortal(newPortal);
        }
        else
        {
            ownerGun.RegisterOrangePortal(newPortal);
        }
    }
    
    Quaternion CalculatePortalRotation(Vector3 normal)
    {
        Quaternion rotation = Quaternion.LookRotation(-normal);
        Vector3 portalRight = Vector3.Cross(Vector3.up, -normal).normalized;
        
        if (portalRight.magnitude > 0.1f)
        {
            Vector3 portalUp = Vector3.Cross(-normal, portalRight).normalized;
            rotation = Quaternion.LookRotation(-normal, portalUp);
        }
        
        return rotation;
    }
}