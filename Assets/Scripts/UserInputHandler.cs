using UnityEngine;
using UnityEngine.UI;

public class UserInputHandler : MonoBehaviour
{
    [SerializeField] private InputField speedInputField;
    [SerializeField] private InputField viscosityInputField;

    public Button applyButton;

    public NewLatticeScript latticeScript;

    void Start()
    {
        applyButton.onClick.AddListener(() => ApplyAllSettings());

        // Adding listeners to handle input fields' On End Edit event
        speedInputField.onEndEdit.AddListener(ApplySpeedSetting);
        viscosityInputField.onEndEdit.AddListener(ApplyViscositySetting);
    }

    // Applies settings for both fields when the apply button is pressed
    public void ApplyAllSettings()
    {
        ApplySpeedSetting(speedInputField.text);
        ApplyViscositySetting(viscosityInputField.text);
    }

    // Applies the speed setting
    public void ApplySpeedSetting(string speedText)
    {
        if (latticeScript != null)
        {
            if (float.TryParse(speedText, out float speed))
            {
                latticeScript.initSpeed = speed;
                Debug.Log($"Speed updated to: {speed}");
            }
            else
            {
                Debug.LogError("Invalid input for speed. Please enter a valid number.");
            }
        }
    }

    // Applies the viscosity setting
    public void ApplyViscositySetting(string viscosityText)
    {
        if (latticeScript != null)
        {
            if (float.TryParse(viscosityText, out float viscosity))
            {
                latticeScript.initViscosity = viscosity;
                Debug.Log($"Viscosity updated to: {viscosity}");
            }
            else
            {
                Debug.LogError("Invalid input for viscosity. Please enter a valid number.");
            }
        }
    }
}