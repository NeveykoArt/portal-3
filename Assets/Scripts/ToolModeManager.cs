using UnityEngine;

public enum ToolMode { Magnet, Portal }

public class ToolModeManager : MonoBehaviour
{
    public static ToolModeManager Instance { get; private set; }
    public ToolMode CurrentMode { get; private set; } = ToolMode.Magnet;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CurrentMode = CurrentMode == ToolMode.Magnet ? ToolMode.Portal : ToolMode.Magnet;
            Debug.Log($"[ToolMode] Режим: {CurrentMode}");
        }
    }

    public void SetMode(ToolMode mode) => CurrentMode = mode;
}