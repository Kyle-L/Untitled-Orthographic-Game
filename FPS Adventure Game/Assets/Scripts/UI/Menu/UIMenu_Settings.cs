using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All player settings related functions needed inside of the menus.
/// </summary>
public class UIMenu_Settings : MonoBehaviour {

    // The UI elements that change the settings.
    [Header("UI Elements")]
    [SerializeField]
    private Slider volumeSlider;
    [SerializeField]
    private Slider moveSlider;
    [SerializeField]
    private Slider heightSlider;
    [SerializeField]
    private Slider rotateSlider;
    [SerializeField]
    private Slider zoomSlider;
    [SerializeField]
    private Dropdown qualityDropdown;

    private void Start() {
        // Set the UI elements to represent the values of settings.
        volumeSlider.value = SettingsController.instance.Volume;
        moveSlider.value = SettingsController.instance.CameraMoveSpeed;
        heightSlider.value = SettingsController.instance.CameraHeightSpeed;
        rotateSlider.value = SettingsController.instance.CameraRotateSpeed;
        zoomSlider.value = SettingsController.instance.CameraZoomSpeed;
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
    /// Changes the camera move sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void MoveSensitivitySlider(Slider aSlider) {
        SettingsController.instance.CameraMoveSpeed = aSlider.value;
    }

    /// <summary>
    /// Changes the camera height sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void HeightSensitivitySlider(Slider aSlider) {
        SettingsController.instance.CameraHeightSpeed = aSlider.value;
    }

    /// <summary>
    /// Changes the camera rotate sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void RotateSensitivitySlider(Slider aSlider) {
        SettingsController.instance.CameraRotateSpeed = aSlider.value;
    }

    /// <summary>
    /// Changes the zoom sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void ZoomSensitivitySlider(Slider aSlider) {
        SettingsController.instance.CameraZoomSpeed = aSlider.value;
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
