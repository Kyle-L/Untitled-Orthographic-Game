using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All player settings related functions needed inside of the menus.
/// </summary>
public class UIMenu_Settings : MonoBehaviour {
    public static UIMenu_Settings instance;

    // The UI elements that change the settings.
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Slider rotateSlider;
    public Slider heightSlider;
    public Slider zoomSlider;
    public Dropdown qualityDropdown;

    private void Awake() {
        #region Enforces Singleton Pattern.
        //Check if instance already exists
        if (instance == null) {
            //if not, set instance to this	
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a MyNetworkManager.
            Destroy(gameObject);
        }
        #endregion
    }

    private void Start() {
        // Set the UI elements to represent the values of settings.
        volumeSlider.value = SettingsController.instance.Volume;
        rotateSlider.value = SettingsController.instance.RotateSensitivity;
        heightSlider.value = SettingsController.instance.HeightSensitivity;
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
    /// Changes the rotate sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void RotateSensitivitySlider(Slider aSlider) {
        SettingsController.instance.RotateSensitivity = aSlider.value;
    }

    /// <summary>
    /// Changes the height sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="aSlider"></param>
    public void HeightSensitivitySlider(Slider aSlider) {
        SettingsController.instance.HeightSensitivity = aSlider.value;
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
