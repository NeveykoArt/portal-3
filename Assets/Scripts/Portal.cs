using UnityEngine;

public class Portal : MonoBehaviour
{
    public PortalColor portalColor;
    
    [SerializeField] private Camera portalCamera;
    [SerializeField] private MeshRenderer portalScreen;
    [SerializeField] private new Collider collider;
    
    private Portal linkedPortal;
    
    void Start()
    {
        if (portalCamera == null) portalCamera = GetComponentInChildren<Camera>();
        if (portalScreen == null) portalScreen = GetComponentInChildren<MeshRenderer>();
        if (collider == null) collider = GetComponent<Collider>();
        
        PortalController.Instance?.RegisterPortal(this);
    }
    
    void OnDestroy() => PortalController.Instance?.UnregisterPortal(this);
    
    public void SetLinkedPortal(Portal portal) => linkedPortal = portal;
    
    public void ClearLink()
    {
        linkedPortal = null;
        portalCamera.enabled = false;
        portalCamera.targetTexture = null;
        portalScreen.enabled = false;
    }
    
    public void SetTargetTexture(RenderTexture texture)
    {
        portalCamera.targetTexture = texture;
        portalCamera.enabled = true;
        
        if (linkedPortal != null)
        {
            portalCamera.transform.position = linkedPortal.transform.position;
            portalCamera.transform.rotation = linkedPortal.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }
    
    public void EnableScreen()
    {
        if (linkedPortal != null)
        {
            portalScreen.material.mainTexture = portalCamera.targetTexture;
            portalScreen.enabled = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (linkedPortal == null) return;
        
        PortalPassenger passenger = other.GetComponent<PortalPassenger>();
        if (passenger != null && passenger.IsPassingThrough) return;
        
        if (other.CompareTag("Player") || other.CompareTag("Movable"))
        {
            TeleportObject(other.gameObject);
        }
    }
    
    void TeleportObject(GameObject obj)
    {
        PortalPassenger passenger = obj.GetComponent<PortalPassenger>();
        if (passenger == null)
            passenger = obj.AddComponent<PortalPassenger>();
        
        passenger.StartPassage(this);
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Vector3 velocity = rb != null ? rb.linearVelocity : Vector3.zero;
        
        // Сохраняем относительную позицию и поворот от ВХОДНОГО портала
        Vector3 relativePos = transform.InverseTransformPoint(obj.transform.position);
        Quaternion relativeRot = Quaternion.Inverse(transform.rotation) * obj.transform.rotation;
        
        // Преобразуем через выходной портал
        Vector3 newPos = linkedPortal.transform.TransformPoint(relativePos);
        
        // КЛЮЧЕВОЕ ИЗМЕНЕНИЕ: Правильное преобразование поворота
        // Вычисляем разницу в повороте между порталами
        Quaternion rotationDiff = linkedPortal.transform.rotation * Quaternion.Inverse(transform.rotation);
        
        // Применяем разницу поворотов к относительному повороту объекта
        Quaternion newRot = rotationDiff * obj.transform.rotation;
        
        // Для игрока корректируем дополнительно, чтобы он смотрел наружу
        bool isPlayer = obj.GetComponentInChildren<Camera>() != null || obj.CompareTag("Player");
        
        if (isPlayer)
        {
            // В Portal игрок выходит лицом в ту же сторону, куда смотрит портал выхода
            // То есть его forward должен совпадать с направлением обзора из портала
            
            // Получаем направление, в котором должен смотреть игрок после выхода
            // Это направление от портала наружу (противоположно forward портала)
            Vector3 desiredForward = -linkedPortal.transform.forward;
            
            // Сохраняем вертикальную составляющую (относительно мира) чтобы не потерять up
            // Но направление взгляда принудительно устанавливаем наружу
            newRot = Quaternion.LookRotation(desiredForward, Vector3.up);
            
            // Дополнительно учитываем, что игрок мог смотреть вверх/вниз при входе
            // Сохраняем угол наклона камеры по X от входа
            float currentPitch = obj.transform.eulerAngles.x;
            newRot = Quaternion.Euler(currentPitch, newRot.eulerAngles.y, 0);
        }
        
        // Смещаем наружу от портала
        Vector3 outwardDirection = -linkedPortal.transform.forward;
        float objectRadius = 0.5f;
        if (obj.TryGetComponent<Collider>(out Collider objCollider))
        {
            objectRadius = objCollider.bounds.extents.magnitude;
        }
        
        Vector3 offset = outwardDirection * (objectRadius + 0.3f);
        newPos += offset;
        
        obj.transform.position = newPos;
        obj.transform.rotation = newRot;
        
        // Преобразуем скорость с той же разницей поворотов
        if (rb != null)
        {
            Vector3 newVelocity = rotationDiff * velocity;
            rb.linearVelocity = newVelocity;
        }
        passenger.CompletePassage();
    }
}