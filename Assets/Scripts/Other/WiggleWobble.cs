using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WiggleWobble : MonoBehaviour
{
    public Material wobbleMaterial;
    [Range(0, 1)]
    public float waveStrength = 0.1f;
    [Range(0, 10)]
    public float waveSpeed = 1.0f;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (wobbleMaterial != null)
        {
            wobbleMaterial.SetFloat("_WaveStrength", waveStrength);
            wobbleMaterial.SetFloat("_WaveSpeed", waveSpeed);
            Graphics.Blit(source, destination, wobbleMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
