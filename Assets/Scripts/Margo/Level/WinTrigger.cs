using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WinTrigger : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string playerTag = "Player";

    private bool triggered = false;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;

        if (LevelManager.Instance != null)
            LevelManager.Instance.Win();
    }
}
