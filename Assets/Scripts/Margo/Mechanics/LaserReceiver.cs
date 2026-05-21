using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    [Header(" Подключение")]
    [SerializeField] private DoorController connectedDoor;

    private bool isPowered = false;

    public void Activate()
    {
        if (!isPowered)
        {
            isPowered = true;
            connectedDoor?.Open();
        }
    }

    public void Deactivate()
    {
        if (isPowered)
        {
            isPowered = false;
            connectedDoor?.Close();
        }
    }
}