using UnityEngine;

public class MagnetSelector : MonoBehaviour
{
    [Header("🧲 Настройки")]
    [SerializeField] private LayerMask cargoLayer;
    [SerializeField] private float magnetRange = 15f;

    private MagnetScript selectedCargo;

    private void Update()
    {
        if (ToolModeManager.Instance != null && ToolModeManager.Instance.CurrentMode != ToolMode.Magnet)
        { ClearSelection(); return; }

        bool isInteracting = Input.GetMouseButton(0) || Input.GetMouseButton(1);
        if (!isInteracting) { ClearSelection(); return; }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, magnetRange, cargoLayer))
        {
            if (hit.collider.TryGetComponent(out MagnetScript cargo))
            {
                if (selectedCargo != cargo) { ClearSelection(); selectedCargo = cargo; selectedCargo.isSelected = true; }
            }
            else ClearSelection();
        }
        else ClearSelection();
    }

    private void ClearSelection()
    {
        if (selectedCargo != null) { selectedCargo.isSelected = false; selectedCargo = null; }
    }
}