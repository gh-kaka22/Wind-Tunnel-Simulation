using UnityEngine;
using UnityEngine.UI;

public class ModelMovementManager : MonoBehaviour
{
    public Dropdown modelDropdown; // Reference to the Dropdown component
    public GameObject[] models; // Array of model GameObjects to select from

    private MonoBehaviour currentModelScript; // The currently selected model's script

    void Start()
    {
        // Add listener to handle dropdown selection
        modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Set the initial dropdown value
        modelDropdown.value = 0;

        // Initially disable all model scripts
        foreach (var model in models)
        {
            DisableAllScripts(model);
        }
    }

    void OnDropdownValueChanged(int index)
    {
        // Disable the currently active model script
        if (currentModelScript != null)
        {
            currentModelScript.enabled = false;
            currentModelScript = null;
        }

        // Ignore the first option (Model Selection)
        if (index == 0)
        {
            return;
        }

        // Adjust index to match models array (since the first dropdown option is a placeholder)
        SelectModel(index - 1);
    }

    public void SelectModel(int index)
    {
        if (index < 0 || index >= models.Length) return; // Ensure the index is within bounds

        // Disable all model scripts to ensure only one is active
        foreach (var model in models)
        {
            DisableAllScripts(model);
        }

        // Enable the selected model's script
        var selectedModel = models[index];
        EnableAllScripts(selectedModel);

        // Store the reference to the currently active script
        currentModelScript = selectedModel.GetComponent<MonoBehaviour>();
    }

    private void DisableAllScripts(GameObject model)
    {
        var scripts = model.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = false;
        }
    }

    private void EnableAllScripts(GameObject model)
    {
        var scripts = model.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            script.enabled = true;
        }
    }
}