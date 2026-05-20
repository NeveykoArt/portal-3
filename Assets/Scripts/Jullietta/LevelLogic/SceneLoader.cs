public static class SceneLoader
{
    private static SceneLoaderMB sceneLoader = null; // ссылка на загрузчик сцен

    public static bool setReference(SceneLoaderMB sceneLoaderReference)
    {
        if (sceneLoader == null)
        {
            sceneLoader = sceneLoaderReference;
            return true;
        }
        return false;
    }

    public static int getLevelsCount() 
    { 
        return sceneLoader.getLevelsCount(); 
    }

    public static void loadMainMenu() 
    { 
        sceneLoader.loadMainMenu(); 
    }

    public static void loadLevel(int index) 
    { 
        sceneLoader.loadLevel(index); 
    }

    public static void loadLevel(string sceneName) 
    { 
        sceneLoader.loadLevel(sceneName); 
    }

    public static void loadNextLevel() 
    { 
        sceneLoader.loadNextLevel(); 
    }

    public static string getLoadingLevelName() 
    { 
        return sceneLoader.getLoadingLevelName(); 
    }

    public static void loadingComplete() 
    { 
        sceneLoader.loadingComplete(); 
    }
}