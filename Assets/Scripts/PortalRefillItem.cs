using UnityEngine;

public class PortalRefillItem : MonoBehaviour
{
    [Header("Настройки пополнения")]
    [SerializeField] private int refillAmount = 1;        // Сколько порталов добавить
    [SerializeField] private bool destroyOnPickup = true;  // Уничтожать ли предмет после поднятия
    
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что подбирает игрок
        if (!other.CompareTag("Player")) return;
        
        // Пополняем порталы
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RefillPortals(refillAmount);
        }
        else
        {
            Debug.LogWarning("LevelManager.Instance not found!");
            return;
        }
        
        // Уничтожаем предмет
        if (destroyOnPickup)
            Destroy(gameObject);
        else
        {
            // Если не уничтожаем, можно отключить коллайдер на время перезарядки
            GetComponent<Collider>().enabled = false;
            // Или просто временно скрыть/отключить
        }
    }
}