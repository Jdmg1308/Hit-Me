using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume; // assign in the Inspector
    private ColorGrading colorGrading;
    public bool rainbowEnabled;
    public float rainbowCycleSpeed = 1.0f; // Speed of the rainbow cycle
    public bool greenEnabled;
    public float greenPulseSpeed = 1.5f; // Speed of the green pulse
    private float pulseTime;

    // Start is called before the first frame update
    void Start()
    {
         // Access the Color Grading effect from the post-processing profile
        postProcessVolume.profile.TryGetSettings(out colorGrading);
        rainbowEnabled = false;
        greenEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (colorGrading != null)
        {
            if (rainbowEnabled)
            {
                // Cycle through colors using a hue shift
                float hue = Mathf.PingPong(Time.time * rainbowCycleSpeed, 1.0f); // Loops between 0 and 1
                Color rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f); // Full saturation and brightness

                // Apply the rainbow color filter
                colorGrading.colorFilter.value = rainbowColor;
            } 
            else if (greenEnabled)
            {
                // Create a pulsating effect using a sine wave
                pulseTime += Time.deltaTime * greenPulseSpeed;
                float intensity = Mathf.Abs(Mathf.Sin(pulseTime)); // Oscillates between 0 and 1
                // Apply the pulse to a green color filter
                colorGrading.colorFilter.value = Color.Lerp(Color.white, Color.green, intensity);   
            }
            else
            {
                // reset the color so we aren't stuck 
                colorGrading.colorFilter.value = Color.white;
            }
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

}
