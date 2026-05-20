using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagnetScript : MonoBehaviour
{
    [Header("🧲 Настройки")]
    [SerializeField] private float pushStrength = 20f;
    [Header("📏 Удержание")]
    [SerializeField] private float holdDistance = 1.5f;
    [SerializeField] private float springStiffness = 25f;
    [SerializeField] private float springDamping = 9f;

    public bool isSelected { get; set; } = false;
    private Rigidbody rb;
    private Camera mainCamera;
    private bool isPulling, isPushing;

    private void Awake() { rb = GetComponent<Rigidbody>(); mainCamera = Camera.main; }

    private void Update()
    {
        // ПРИНУДИТЕЛЬНО сбрасываем выделение при смене режима
        if (ToolModeManager.Instance != null && ToolModeManager.Instance.CurrentMode != ToolMode.Magnet)
        { isSelected = false; isPulling = false; isPushing = false; return; }

        if (!isSelected) { isPulling = false; isPushing = false; return; }
        isPulling = Input.GetMouseButton(0);
        isPushing = Input.GetMouseButton(1);
    }

    private void FixedUpdate()
    {
        if (!isSelected) { rb.useGravity = true; return; }

        if (isPushing)
        {
            rb.useGravity = true;
            rb.AddForce(mainCamera.transform.forward * pushStrength, ForceMode.Acceleration);
        }
        else if (isPulling)
        {
            rb.useGravity = false;
            Vector3 targetPos = mainCamera.transform.position + mainCamera.transform.forward * holdDistance;
            Vector3 error = targetPos - transform.position;
            rb.AddForce(error * springStiffness - rb.linearVelocity * springDamping, ForceMode.Acceleration);
        }
        else rb.useGravity = true;
    }
}