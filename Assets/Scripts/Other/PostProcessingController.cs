using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume; // Assign in the Inspector
    // Wavy Wobble Effect
    public WiggleWobble wiggleWobble; // Reference to the wigglinig script
    private ColorGrading colorGrading;

    // LSD effect parameters
    public bool rainbowEnabled;
    public float rainbowCycleSpeed = 1.0f; // Speed of the rainbow cycle

    // High (weed) effect parameters
    public bool greenEnabled;
    public float greenPulseSpeed = 1.5f; // Speed of the green pulse
    private float pulseTime;

    // Drunk effect parameters
    public bool isDrunk;
    public float wobbleAmount = 0.1f; // Amount of wobble
    public float wobbleSpeed = 2f; // Speed of wobble

    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out colorGrading);
        
        // Start with effects disabled
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
        if (colorGrading != null)
        {
            if (rainbowEnabled)
            {
                // Cycle through colors using a hue shift
                float hue = Mathf.PingPong(Time.time * rainbowCycleSpeed, 1.0f);
                Color rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
                colorGrading.colorFilter.value = rainbowColor;
            } 
            else if (greenEnabled)
            {
                // Pulsating green effect
                pulseTime += Time.deltaTime * greenPulseSpeed;
                float intensity = Mathf.Abs(Mathf.Sin(pulseTime));
                colorGrading.colorFilter.value = Color.Lerp(Color.white, Color.green, intensity);
            }
            else
            {
                colorGrading.colorFilter.value = Color.white; // Reset to white
            }
        }

        // Handle drunk effect
        if (wiggleWobble != null)
        {
            wiggleWobble.enabled = isDrunk;
        }
    }

    public void ToggleGreenEffect(bool isEnabled)
    {
        greenEnabled = isEnabled;
    }

    public void ToggleRainbowEffect(bool isEnabled)
    {
        rainbowEnabled = isEnabled;
    }

    public void ToggleDrunkEffect(bool isEnabled)
    {
        isDrunk = isEnabled;
    }
}
