using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIAudioButton : MonoBehaviour
{
    public AudioClip clickSound;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (AudioManager.Instance != null && clickSound != null)
            AudioManager.Instance.PlayButtonClick(clickSound);
    }
}