using UnityEngine;

public class LaserReflector : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Сдвиг точки отражения от поверхности, чтобы луч не застревал")]
    [SerializeField] private float surfaceOffset = 0.05f;

    // Скрипт не требует Update. 
    // Вся обработка отражения происходит в LaserEmitter через TryGetComponent
}