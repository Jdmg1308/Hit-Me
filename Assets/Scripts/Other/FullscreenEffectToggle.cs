using UnityEngine;

public class FullscreenEffectToggle : MonoBehaviour
{
    public Material effect1Material; // First fullscreen effect
    public Material effect2Material; // Second fullscreen effect
    private Material currentMaterial; // The currently active material

    void Start()
    {
        // Start with no effect
        currentMaterial = null;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Debug.Log("OnRenderImage called");
        Graphics.Blit(src, dest, currentMaterial != null ? currentMaterial : null);
    }


    void Update()
    {
        // Toggle Effect 1 with Key "1"
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMaterial = effect1Material;
            Debug.Log("Effect 1 enabled");
        }

        // Toggle Effect 2 with Key "2"
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMaterial = effect2Material;
            Debug.Log("Effect 2 enabled");
        }

        // Disable all effects with Key "0"
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentMaterial = null;
            Debug.Log("All effects disabled");
        }
    }
}
