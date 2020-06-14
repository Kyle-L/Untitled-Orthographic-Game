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
        volumeSlider.SetValueWithoutNotify(SettingsController.instance.Volume);
        volumeSlider.value = SettingsController.instance.Volume;
        rotateSlider.SetValueWithoutNotify(SettingsController.instance.RotateSensitivity);
        heightSlider.SetValueWithoutNotify(SettingsController.instance.HeightSensitivity);
        zoomSlider.SetValueWithoutNotify(SettingsController.instance.ZoomSensitivity);
        qualityDropdown.SetValueWithoutNotify(SettingsController.instance.QualityLevel);
    }

    /// <summary>
    /// Changes the volume based on the value given by a slider.
    /// </summary>
    /// <param name="value"></param>
    public void VolumeSlider(float value) {
        SettingsController.instance.Volume = value;
    }

    /// <summary>
    /// Changes the rotate sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="value"></param>
    public void RotateSensitivitySlider(float value) {
        SettingsController.instance.RotateSensitivity = value;
    }

    /// <summary>
    /// Changes the height sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="value"></param>
    public void HeightSensitivitySlider(float value) {
        SettingsController.instance.HeightSensitivity = value;
    }

    /// <summary>
    /// Changes the zoom sensitivty based on the value given by
    /// a slider.
    /// </summary>
    /// <param name="value"></param>
    public void ZoomSensitivitySlider(float value) {
        SettingsController.instance.ZoomSensitivity = value;
    }

    /// <summary>
    /// Changes the quality level based on the value given by
    /// the dropdown.
    /// </summary>
    /// <param name="value"></param>
    public void QualityLevelSlider(int value) {
        SettingsController.instance.QualityLevel = value;
    }

}
