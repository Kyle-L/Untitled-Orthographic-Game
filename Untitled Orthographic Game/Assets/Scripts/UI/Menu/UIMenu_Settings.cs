using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All player settings related functions needed inside of the menus.
/// </summary>
public class UIMenu_Settings : MonoBehaviour {

    // The UI elements that change the settings.
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Slider mouseSlider;
    public Slider zoomSlider;
    public Dropdown qualityDropdown;

    private void Start() {
        // Set the UI elements to represent the values of settings.
        volumeSlider.value = SettingsController.instance.Volume;
        mouseSlider.value = SettingsController.instance.MouseSensitivity;
        zoomSlider.value = SettingsController.instance.ZoomSensitivity;
        qualityDropdown.value = SettingsController.instance.QualityLevel;
    }

    /// <summary>
    /// Changes the volume based on the value given by a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void VolumeSlider(Slider aSlider) {
        SettingsController.instance.Volume = aSlider.value;
    }

    /// <summary>
    /// Changes the mouse sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void MouseSensitivitySlider(Slider aSlider) {
        SettingsController.instance.MouseSensitivity = aSlider.value;
    }

    /// <summary>
    /// Changes the zoom sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void ZoomSensitivitySlider(Slider aSlider) {
        SettingsController.instance.ZoomSensitivity = aSlider.value;
    }

    /// <summary>
    /// Changes the quality level based on the value given by
    /// the dropdown.
    /// </summary>
    /// <param name="aDropdown"></param>
    public void QualityLevelSlider(Dropdown aDropdown) {
        SettingsController.instance.QualityLevel = aDropdown.value;
    }

}
