// AdvancedFlashlight.cs
using UnityEngine;

public class AdvancedFlashlight : MonoBehaviour
{
    // --- Public Settings (Adjust in Inspector) ---
    [Header("Light Component")]
    [SerializeField] private Light flashlight; // Assign your Spot Light here

    [Header("Transition Speed")]
    [SerializeField] private float transitionSpeed = 10f;

    [Header("Default Mode Settings (Wide Beam)")]
    [SerializeField] private float defaultIntensity = 1.5f;
    [SerializeField] private float defaultRange = 20f;
    [SerializeField] private float defaultSpotAngle = 60f;

    [Header("Focused Mode Settings (Narrow Beam)")]
    [SerializeField] private float focusedIntensity = 5f;
    [SerializeField] private float focusedRange = 50f;
    [SerializeField] private float focusedSpotAngle = 15f;


    // --- Private variables for smoothing ---
    private float targetIntensity;
    private float targetRange;
    private float targetSpotAngle;

    void Awake()
    {
        // Safety check to ensure the light is assigned
        if (flashlight == null)
        {
            Debug.LogError("Flashlight component not assigned in the Inspector!", this);
            enabled = false; // Disable the script if no light is assigned
            return;
        }

        // Set the initial state to default
        SetMode(false); // false = not focused
    }

    void Update()
    {
        // Check if the light is active before processing
        if (!flashlight.gameObject.activeSelf)
        {
            return;
        }

        // Check for right-click hold. GetMouseButton(1) is for the right mouse button.
        bool isFocusing = Input.GetMouseButton(1);
        SetMode(isFocusing);

        // Smoothly interpolate the light's properties towards the target values
        flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);
        flashlight.range = Mathf.Lerp(flashlight.range, targetRange, Time.deltaTime * transitionSpeed);
        flashlight.spotAngle = Mathf.Lerp(flashlight.spotAngle, targetSpotAngle, Time.deltaTime * transitionSpeed);
    }

    // This function sets the target values based on the desired mode
    private void SetMode(bool isFocusing)
    {
        if (isFocusing)
        {
            // Set targets for Focused Mode
            targetIntensity = focusedIntensity;
            targetRange = focusedRange;
            targetSpotAngle = focusedSpotAngle;
        }
        else
        {
            // Set targets for Default Mode
            targetIntensity = defaultIntensity;
            targetRange = defaultRange;
            targetSpotAngle = defaultSpotAngle;
        }
    }
}