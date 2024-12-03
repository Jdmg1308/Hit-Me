using UnityEngine;

public class OverlayManager : MonoBehaviour
{
    public Camera overlayCamera; // Secondary camera for overlays
    public LayerMask overlay1Layer; // Layer for Overlay1
    public LayerMask overlay2Layer; // Layer for Overlay2

    public GameObject overlay1Object; // GameObject for Overlay1
    public GameObject overlay2Object; // GameObject for Overlay2

    private int currentOverlay = 0; // 0 = None, 1 = Overlay1, 2 = Overlay2

void Update()
{


// Toggle Overlay1 with the "1" key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetOverlay(1); // Enable Overlay1
        }

        // Toggle Overlay2 with the "2" key
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetOverlay(2); // Enable Overlay2
        }

        // Disable all overlays with the "0" key
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetOverlay(0); // Disable overlays
        }
}

    public void SetOverlay(int overlay)
    {
        currentOverlay = overlay;

        switch (currentOverlay)
        {
            case 0: // No overlay
                overlayCamera.cullingMask = 0; // Disable rendering
                overlay1Object.SetActive(false);
                overlay2Object.SetActive(false);
                break;

            case 1: // Overlay1
                overlayCamera.cullingMask = overlay1Layer;
                overlay1Object.SetActive(true);
                overlay2Object.SetActive(false);
                break;

            case 2: // Overlay2
                overlayCamera.cullingMask = overlay2Layer;
                overlay1Object.SetActive(false);
                overlay2Object.SetActive(true);
                break;
        }
    }
}
