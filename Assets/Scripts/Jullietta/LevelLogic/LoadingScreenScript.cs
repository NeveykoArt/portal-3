using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenScript : MonoBehaviour
{
    [SerializeField] private Slider progressBar;       // полоса прогресса загрузки
    [SerializeField] private float minDisplayTime = 1f; // настройка времени загрузки

    private void Start()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(LoadLevelAsync());
    }
        private IEnumerator LoadLevelAsync()
    {
        string sceneToLoad = SceneLoader.getLoadingLevelName();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // 1. Анимация заполнения слайдера за фиксированное время
        float startTime = Time.unscaledTime;
        float currentProgress = 0f;
        while (currentProgress < 1f)
        {
            float elapsed = Time.unscaledTime - startTime;
            currentProgress = Mathf.Clamp01(elapsed / minDisplayTime);
            progressBar.value = currentProgress;
            yield return null;
        }

        // 2. Ждём, пока реальная загрузка не достигнет 0.9 (почти завершена)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 3. Убеждаемся, что слайдер показывает 1
        progressBar.value = 1f;

        // 4. Короткая пауза для комфорта (опционально)
        yield return new WaitForSecondsRealtime(0.3f);

        // 5. Сообщаем загрузчику, что уровень полностью загружен (сохраняем прогресс)
        SceneLoader.loadingComplete();

        // 6. Разрешаем активацию сцены
        asyncLoad.allowSceneActivation = true;
    }
}