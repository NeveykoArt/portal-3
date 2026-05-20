using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("🔗 Подключение")]
    [SerializeField] private DoorController connectedDoor;
    private int activeCargos = 0;

    private void Awake() => GetComponent<Collider>().isTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        // 🔑 Заменён PlayerController на PlayerMovement
        if (other.TryGetComponent<MagnetScript>(out _) || other.TryGetComponent<PlayerMovement>(out _))
        {
            activeCargos++;
            if (activeCargos == 1) connectedDoor?.Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MagnetScript>(out _) || other.TryGetComponent<PlayerMovement>(out _))
        {
            activeCargos--;
            if (activeCargos <= 0) connectedDoor?.Close();
        }
    }
}