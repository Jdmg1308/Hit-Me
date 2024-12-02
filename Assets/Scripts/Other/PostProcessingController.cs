using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    public Volume volume; // Assign this in the Inspector
    private ColorAdjustments colorAdjustments;

    // LSD effect parameters
    public bool rainbowEnabled;
    public float rainbowCycleSpeed = 1.0f;

    // High (weed) effect parameters
    public bool greenEnabled;
    public float greenPulseSpeed = 1.5f;
    private float pulseTime;

    // Drunk effect parameters
    public bool isDrunk;
    public WiggleWobble wiggleWobble; // Reference to your wobble script
    public float wobbleAmount = 0.1f;
    public float wobbleSpeed = 2f;

    void Start()
    {
        // Access Color Adjustments in URP
        volume.profile.TryGet(out colorAdjustments);

        // Initialize all effects as disabled
        rainbowEnabled = false;
        greenEnabled = false;
        isDrunk = false;

        if (wiggleWobble != null)
        {
            wiggleWobble.enabled = false; // Disable wobble effect initially
        }
    }

    void Update()
    {
        // Handle color grading effects
        if (colorAdjustments != null)
        {
            if (rainbowEnabled)
            {
                float hue = Mathf.PingPong(Time.time * rainbowCycleSpeed, 1.0f);
                Color rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
                colorAdjustments.colorFilter.value = rainbowColor;
            }
            else if (greenEnabled)
            {
                pulseTime += Time.deltaTime * greenPulseSpeed;
                float intensity = Mathf.Abs(Mathf.Sin(pulseTime));
                colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.green, intensity);
            }
            else
            {
                colorAdjustments.colorFilter.value = Color.white;
            }
        }

        // Handle the drunk wobble effect
        if (wiggleWobble != null)
        {
            wiggleWobble.enabled = isDrunk;

            if (isDrunk)
            {
                wiggleWobble.waveStrength = wobbleAmount;
                wiggleWobble.waveSpeed = wobbleSpeed;
            }
        }
    }

    public void ToggleGreenEffect(bool isEnabled) => greenEnabled = isEnabled;

    public void ToggleRainbowEffect(bool isEnabled) => rainbowEnabled = isEnabled;

    public void ToggleDrunkEffect(bool isEnabled) => isDrunk = isEnabled;
}
