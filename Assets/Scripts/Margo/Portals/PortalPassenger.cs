using UnityEngine;

public class PortalPassenger : MonoBehaviour
{
    public bool IsPassingThrough { get; private set; }
    public Portal CurrentPortal { get; private set; }
    
    public void StartPassage(Portal portal)
    {
        IsPassingThrough = true;
        CurrentPortal = portal;
    }
    
    public void CompletePassage()
    {
        IsPassingThrough = false;
        CurrentPortal = null;
    }
}