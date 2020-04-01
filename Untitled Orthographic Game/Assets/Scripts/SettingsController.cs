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
    private float defaultRotate = 2.5f;
    [SerializeField]
    private float defaultHeight = 2.5f;
    [SerializeField]
    private float defaultZoom = 1.5f;
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
        RotateSensitivity = PlayerPrefs.GetFloat("rotateSensitivity", defaultRotate);
        HeightSensitivity = PlayerPrefs.GetFloat("heightSensitivity", defaultHeight);
        ZoomSensitivity = PlayerPrefs.GetFloat("zoomSensitivity", defaultZoom);
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
    /// Changes the rotate sensitivity.
    /// </summary>
    public float RotateSensitivity {
        get { return rotateSensitivity; }
        set {
            PlayerPrefs.SetFloat("rotateSensitivity", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraRotateSpeed = value;
            }

            rotateSensitivity = value;
        }
    }
    private float rotateSensitivity;

    /// <summary>
    /// Changes the height sensitivity.
    /// </summary>
    public float HeightSensitivity {
        get { return heightSensitivity; }
        set {
            PlayerPrefs.SetFloat("heightSensitivity", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraHeightSpeed = value;
            }

            heightSensitivity = value;
        }
    }
    private float heightSensitivity;

    /// <summary>
    /// Changes the zoom sensitivity.
    /// </summary>
    public float ZoomSensitivity {
        get { return zoomSensitivity; }
        set {
            PlayerPrefs.SetFloat("zoomSensitivity", value);
            if (CameraController.instance != null) {
                CameraController.instance.cameraZoomSpeed = value;
            }

            zoomSensitivity = value;
        }
    }
    private float zoomSensitivity;

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
