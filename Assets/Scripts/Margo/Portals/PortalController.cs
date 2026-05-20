using UnityEngine;
using System.Collections.Generic;

public class PortalController : MonoBehaviour
{
    public static PortalController Instance { get; private set; }
    
    [Header("Render Textures")]
    [SerializeField] private RenderTexture bluePortalRT;
    [SerializeField] private RenderTexture orangePortalRT;
    
    private List<Portal> portals = new List<Portal>();
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void RegisterPortal(Portal portal)
    {
        if (!portals.Contains(portal))
        {
            portals.Add(portal);
            CheckAndLinkPortals();
        }
    }
    
    public void UnregisterPortal(Portal portal)
    {
        if (portals.Contains(portal))
        {
            portals.Remove(portal);
            
            foreach (var p in portals)
                p.ClearLink();
        }
    }
    
    public void CheckAndLinkPortals()
    {
        if (portals.Count == 2)
            LinkPortals();
    }
    
    void LinkPortals()
    {
        Portal bluePortal = null;
        Portal orangePortal = null;
        
        foreach (var portal in portals)
        {
            if (portal.portalColor == PortalColor.Blue)
                bluePortal = portal;
            else if (portal.portalColor == PortalColor.Orange)
                orangePortal = portal;
        }
        
        if (bluePortal == null || orangePortal == null) return;
        
        bluePortal.SetLinkedPortal(orangePortal);
        orangePortal.SetLinkedPortal(bluePortal);
        
        bluePortal.SetTargetTexture(orangePortalRT);
        orangePortal.SetTargetTexture(bluePortalRT);
        
        bluePortal.EnableScreen();
        orangePortal.EnableScreen();
    }
}