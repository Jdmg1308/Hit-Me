using UnityEngine;

public class WiggleWobbleController : MonoBehaviour
{
    public Material wiggleWobbleMaterial;
    public float customTimeScale = 1.0f;

    private void Update()
    {
        if (wiggleWobbleMaterial != null)
        {
            float scaledTime = Time.time * customTimeScale;
            wiggleWobbleMaterial.SetFloat("_TimeScale", scaledTime);
        }
    }
}
