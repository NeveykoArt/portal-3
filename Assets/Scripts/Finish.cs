using UnityEngine;

public class WinTrigger3D : MonoBehaviour
{
    [Header("Настройки триггера")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool destroyAfterWin = false; // Уничтожить триггер после победы
    
    private MenuManager menuManager;
    
    private void Start()
    {
        // Находим MenuManager на сцене
        menuManager = FindFirstObjectByType<MenuManager>();
        
        // Убеждаемся, что это 3D коллайдер
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошедший объект - игрок
        if (other.CompareTag(playerTag) && menuManager != null)
        {
            menuManager.OnPlayerWin(other.gameObject);
            
            // Уничтожаем триггер, чтобы не срабатывал повторно
            if (destroyAfterWin)
            {
                Destroy(gameObject);
            }
        }
    }
}