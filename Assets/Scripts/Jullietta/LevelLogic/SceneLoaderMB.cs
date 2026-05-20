using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderMB : MonoBehaviour
{
    [Header("Utility Scenes")]
    [SerializeField] private string mainMenu;
    [SerializeField] private string loadingScreen;

    [Header("Levels")]
    [SerializeField] private List<string> levels = new List<string>();

    private int currentLevelIndex = -2;
    private int loadingLevelIndex = -2;
    private int highestLvlIndex = 0;

    private static SceneLoaderMB instance;

    // Свойство для доступа из других скриптов (например, с экрана загрузки)
    public static SceneLoaderMB Instance => instance;

    private void Awake()
    {
        SceneLoader.setReference(this);
        // Реализация синглтона
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Загрузка сохранённого прогресса
        highestLvlIndex = PlayerPrefs.GetInt("HighestLvlIndex", 0);

        currentLevelIndex = -2;
        loadingLevelIndex = -2;
    }

    /// <summary>Возвращает количество уровней в списке.</summary>
    public int getLevelsCount() => levels.Count;

    /// <summary>Возвращает наибольший индекс посещённого уровня.</summary>
    public int highscoreLvl() => highestLvlIndex;

    /// <summary>Загрузить главное меню (через экран загрузки).</summary>
    public void loadMainMenu()
    {
        currentLevelIndex = -2;
        loadingLevelIndex = -2;
        SceneManager.LoadScene(loadingScreen);
    }

    /// <summary>Загрузить уровень по индексу.</summary>
    public void loadLevel(int index)
    {
        index = index - 2;
        if (index < 0 || index >= levels.Count) return;

        currentLevelIndex = index;
        loadingLevelIndex = index;
        SceneManager.LoadScene(loadingScreen);
    }

    /// <summary>Загрузить уровень по имени сцены.</summary>
    public void loadLevel(string sceneName)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] == sceneName)
            {
                loadingLevelIndex = i;
                currentLevelIndex = i;
                SceneManager.LoadScene(loadingScreen);
                return;
            }
        }
        Debug.LogWarning($"Уровень с именем {sceneName} не найден!");
    }

    /// <summary>Загрузить следующий уровень (или главное меню, если уровень последний).</summary>
    public void loadNextLevel()
    {
        if (currentLevelIndex + 1 >= levels.Count)
        {
            loadMainMenu();
        }
        else
        {
            loadingLevelIndex = currentLevelIndex + 1;
            SceneManager.LoadScene(loadingScreen);
        }
    }
    public string getLoadingLevelName()    // метод получения имени сцены, которую следует загрузить
    {
        if (loadingLevelIndex == -2) return mainMenu;

        if (loadingLevelIndex < -2 || loadingLevelIndex >= levels.Count) return null;

        return levels[loadingLevelIndex];
    }
    /// <summary>
    /// Вызывается из скрипта экрана загрузки после полной загрузки целевой сцены.
    /// Обновляет текущий индекс и сохраняет прогресс.
    /// </summary>
    public void loadingComplete()
    {
        currentLevelIndex = loadingLevelIndex;

        if (currentLevelIndex > highestLvlIndex)
        {
            highestLvlIndex = currentLevelIndex;
            PlayerPrefs.SetInt("HighestLvlIndex", highestLvlIndex);
            PlayerPrefs.Save();
        }
    }
}