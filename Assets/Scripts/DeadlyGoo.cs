using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeadlyGoo : MonoBehaviour
{
    [Header("⚙️ Настройки")]
    [SerializeField] private float deathDelay = 1.0f;       // Задержка перед рестартом
    [SerializeField] private bool destroyCubes = true;      // Уничтожать ли упавшие кубы

    private Collider gooCollider;
    private bool isTriggered = false;

    private void Start()
    {
        gooCollider = GetComponent<Collider>();
        if (gooCollider != null) gooCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(HandlePlayerDeath());
        }
        else if (other.CompareTag("Movable"))
        {
            // Кубы просто уничтожаем, чтобы не забивать физику
            if (destroyCubes) Destroy(other.gameObject);
            else other.gameObject.SetActive(false);
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        // Отключаем коллайдер, чтобы не срабатывало повторно при анимации падения
        if (gooCollider != null) gooCollider.enabled = false;

        // Ждём, чтобы игрок успел "провалиться" визуально
        yield return new WaitForSeconds(deathDelay);

        // Пытаемся вызвать ваше меню проигрыша
        var menuManager = FindFirstObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ShowLoseMenu();
        }
        else
        {
            // Фолбэк: мгновенная перезагрузка сцены
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}