using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadlyGoo : MonoBehaviour
{
    [Header("⚙️ Настройки")]
    [SerializeField] private float deathDelay = 0.5f; // Небольшая задержка, чтобы игрок "провалился"
    [SerializeField] private bool destroyCubes = true;

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
            Invoke(nameof(ReloadScene), deathDelay);
        }
        else if (other.CompareTag("Movable"))
        {
            if (destroyCubes) Destroy(other.gameObject);
            else other.gameObject.SetActive(false);
        }
    }

    private void ReloadScene()
    {
        // Пробуем использовать ваш менеджер сцен
        var sceneManager = FindFirstObjectByType<SceneManagerScript>();
        if (sceneManager != null)
        {
            sceneManager.LoadSceneByName(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Фолбэк: прямая перезагрузка
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}