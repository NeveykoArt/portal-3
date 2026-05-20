using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    [Header("🔗 Подключение")]
    [SerializeField] private ActivatableDevice connectedDevice; // Было: IActivatable
    [SerializeField] private Material receiverMaterial; // (если нужно)

    private bool isPowered = false;

    public void Activate()
    {
        if (!isPowered)
        {
            isPowered = true;
            connectedDevice?.Open(); // ← Обновлён вызов
        }
    }

    public void Deactivate()
    {
        if (isPowered)
        {
            isPowered = false;
            connectedDevice?.Close(); // ← Обновлён вызов
        }
    }
}