using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Метод вызывается при нажатии на кнопку
    public void QuitGame()
    {
        // Если приложение запущено в редакторе, просто выводим сообщение
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}