using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerTutorialZone : MonoBehaviour
{
    [TextArea(2, 6)]
    public string message;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool destroyAfterTrigger = false;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (TutorialHintDisplay.Instance != null)
            TutorialHintDisplay.Instance.Show(message);

        if (destroyAfterTrigger)
            Destroy(gameObject);
    }
}
