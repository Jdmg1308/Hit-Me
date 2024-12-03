using UnityEngine;

public class ToggleFullscreenEffect : MonoBehaviour
{
    public Material effectMaterial1; // Material for effect 1
    public Material effectMaterial2; // Material for effect 2

    void Update()
    {
        // Enable Effect 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FullscreenEffectRendererFeature.instance?.SetEffectMaterial(effectMaterial1);
            Debug.Log("Effect 1 Enabled");
        }

        // Enable Effect 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FullscreenEffectRendererFeature.instance?.SetEffectMaterial(effectMaterial2);
            Debug.Log("Effect 2 Enabled");
        }

        // Disable All Effects
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FullscreenEffectRendererFeature.instance?.SetEffectMaterial(null);
            Debug.Log("Effects Disabled");
        }
    }
}
