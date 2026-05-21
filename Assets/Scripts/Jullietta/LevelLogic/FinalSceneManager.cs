using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FinalSceneManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private float originalMusicVolume;
    private float originalSFXVolume;

    private void Start()
    {
        // Получаем VideoPlayer с этого объекта или его дочерних объектов
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = GetComponentInChildren<VideoPlayer>();
        }

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer не найден на FinalScene!");
            return;
        }

        // Отключаем музыку и SFX
        MuteAudio();

        // Подписываемся на событие окончания видео
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void MuteAudio()
    {
        if (AudioManager.Instance != null)
        {
            // Сохраняем текущие громкости
            originalMusicVolume = AudioManager.Instance.musicVolume;
            originalSFXVolume = AudioManager.Instance.sfxVolume;

            // Отключаем звуки
            AudioManager.Instance.SetMusicVolume(0f);
            AudioManager.Instance.SetSFXVolume(0f);
        }
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        // Загружаем главное меню
        if (SceneLoaderMB.Instance != null)
        {
            SceneLoaderMB.Instance.loadMainMenu();
        }
        else
        {
            // Fallback, если SceneLoaderMB недоступен
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        // Восстанавливаем громкость при выходе со сцены (на случай, если сцена закроется другим способом)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(originalMusicVolume);
            AudioManager.Instance.SetSFXVolume(originalSFXVolume);
        }
    }
}
