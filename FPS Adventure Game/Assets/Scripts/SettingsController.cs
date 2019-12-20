using UnityEngine;

/// <summary>
/// A general controller for all of the settings
/// dictated by a user.
/// </summary>
public class SettingsController : MonoBehaviour {
    public static SettingsController instance;

    // Default values of the settings.
    [Header("Default Settings")]
    [SerializeField]
    private float defaultCameraMoveSpeed = -1f;
    [SerializeField]
    private float defaultCameraHeightSpeed = 2f;
    [SerializeField]
    private float defaultCameraRotateSpeed = 20f;
    [SerializeField]
    private float defaultCameraZoomSpeed = 2f;
    [SerializeField]
    private float defaultVolume = 2.5f;

    public void Awake() {
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

        /* Gets the values stored in player prefs, otherwise the default
         * values are used if the player has not changed the settings. */
        Volume = PlayerPrefs.GetFloat("volume", defaultVolume);
        CameraMoveSpeed = PlayerPrefs.GetFloat("cameraMoveSpeed", defaultCameraMoveSpeed);
        CameraHeightSpeed = PlayerPrefs.GetFloat("cameraHeightSpeed", defaultCameraHeightSpeed);
        CameraRotateSpeed = PlayerPrefs.GetFloat("cameraRotateSpeed", defaultCameraRotateSpeed);
        CameraZoomSpeed = PlayerPrefs.GetFloat("cameraZoomSpeed", defaultCameraZoomSpeed);
        if (PlayerPrefs.GetInt("qualityLevel") != QualitySettings.GetQualityLevel()) {
            QualityLevel = QualitySettings.GetQualityLevel();
        } else {
            QualityLevel = PlayerPrefs.GetInt("qualityLevel", QualitySettings.GetQualityLevel());
        }
    }

    /// <summary>
    /// Changes the volume.
    /// </summary>
    public float Volume {
        get { return volume; }
        set {
            PlayerPrefs.SetFloat("volume", value);
            AudioListener.volume = volume;
            volume = value;
        }
    }
    private float volume;

    /// <summary>
    /// Changes the mouse sensitivity.
    /// </summary>
    public float CameraRotateSpeed {
        get { return cameraRotateSpeed; }
        set {
            PlayerPrefs.SetFloat("cameraRotateSpeed", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraRotateSpeed = value;
            }
            cameraRotateSpeed = value;
        }
    }
    private float cameraRotateSpeed;

    public float CameraHeightSpeed {
        get { return cameraHeightSpeed; }
        set {
            PlayerPrefs.SetFloat("cameraHeightSpeed", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraHeightSpeed = value;
            }
            cameraHeightSpeed = value;
        }
    }
    private float cameraHeightSpeed;

    public float CameraMoveSpeed {
        get { return cameraMoveSpeed; }
        set {
            PlayerPrefs.SetFloat("cameraHeightSpeed", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraMoveSpeed = value;
            }
            cameraMoveSpeed = value;
        }
    }
    private float cameraMoveSpeed;

    /// <summary>
    /// Changes the zoom sensitivity.
    /// </summary>
    public float CameraZoomSpeed {
        get { return cameraZoomSpeed; }
        set {
            PlayerPrefs.SetFloat("zoomSensitivity", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraZoomSpeed = value;
            }
            cameraZoomSpeed = value;
        }
    }
    private float cameraZoomSpeed;

    /// <summary>
    /// Changes the quality level.
    /// </summary>
    public int QualityLevel {
        get { return qualityLevel; }
        set {
            PlayerPrefs.SetInt("qualityLevel", value);
            QualitySettings.SetQualityLevel(value);
            qualityLevel = value;
        }
    }
    private int qualityLevel;
}
