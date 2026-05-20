using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("🔗 Подключение")]
    [SerializeField] private ActivatableDevice connectedDevice; // Было: IActivatable или DoorController
    private int activeCargos = 0;

    private void Awake() => GetComponent<Collider>().isTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MagnetScript>(out _) || other.TryGetComponent<PlayerMovement>(out _))
        {
            activeCargos++;
            if (activeCargos == 1) connectedDevice?.Open(); // ← Обновлён вызов
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MagnetScript>(out _) || other.TryGetComponent<PlayerMovement>(out _))
        {
            activeCargos--;
            if (activeCargos <= 0) connectedDevice?.Close(); // ← Обновлён вызов
        }
    }
}