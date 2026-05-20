using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FirstPersonCamera : MonoBehaviour
{
    [Header("🎮 Настройки")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float lookUpLimit = 80f;
    [SerializeField] private float lookDownLimit = -80f;
    [Header("🔗 Ссылки")]
    [SerializeField] private Transform playerBody;

    public Vector3 AimPoint { get; private set; } // 🔑 Нужно их PortalGun
    private float xRotation = 0f;

    private void Start()
    {
        if (playerBody == null && transform.parent != null) playerBody = transform.parent.parent;
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 50f)) AimPoint = hit.point;
        else AimPoint = ray.origin + ray.direction * 50f;
    }

    private void LateUpdate()
    {
        HandleMouseLook();
        HandleEscape();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY; xRotation = Mathf.Clamp(xRotation, lookDownLimit, lookUpLimit);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (playerBody != null) playerBody.Rotate(Vector3.up * mouseX);
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }
}