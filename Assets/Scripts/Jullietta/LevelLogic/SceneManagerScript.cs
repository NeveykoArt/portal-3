using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour
{
    private GameObject currentLoadingScreen;

    public void Awake()
    {
        unfrezeScene();
    }

    public void frezeScene() => Time.timeScale = 0.0f;
    public void unfrezeScene() => Time.timeScale = 1.0f;

    // Старый метод теперь работает асинхронно с экраном загрузки
    public void LoadSceneByIndex(int sceneIndex)
    {
        unfrezeScene();
        SceneLoader.loadLevel(sceneIndex);
    }
    public void LoadSceneByName(string sceneName)
    {
        unfrezeScene();
        SceneLoader.loadLevel(sceneName);
    }
    public void LoadMainMenu()
    {
        unfrezeScene();
        SceneLoader.loadMainMenu();
    }
}