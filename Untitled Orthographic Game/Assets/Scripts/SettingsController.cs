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

    private const string VOLUME = "volume";
    private const string ROTATE = "rotateSensitivity";
    private const string HEIGHT = "heightSensitivity";
    private const string ZOOM = "zoomSensitivity";
    private const string QUALITY = "qualityLevel";

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
        volume = PlayerPrefs.GetFloat(VOLUME, defaultVolume);
        rotateSensitivity = PlayerPrefs.GetFloat(ROTATE, defaultRotate);
        heightSensitivity = PlayerPrefs.GetFloat(HEIGHT, defaultHeight);
        zoomSensitivity = PlayerPrefs.GetFloat(ZOOM, defaultZoom);
        qualityLevel = PlayerPrefs.GetInt(QUALITY, QualitySettings.GetQualityLevel());
    }

    public void Start() {
        Volume = volume;
        RotateSensitivity = rotateSensitivity;
        HeightSensitivity = heightSensitivity;
        ZoomSensitivity = zoomSensitivity;
        QualityLevel = qualityLevel;
    }

    /// <summary>
    /// Changes the volume.
    /// </summary>
    public float Volume {
        get { return volume; }
        set {
            PlayerPrefs.SetFloat(VOLUME, value);
            PlayerPrefs.Save();
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
            PlayerPrefs.SetFloat(ROTATE, value);
            PlayerPrefs.Save();
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
            PlayerPrefs.SetFloat(HEIGHT, value);
            PlayerPrefs.Save();
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
            PlayerPrefs.SetFloat(ZOOM, value);
            PlayerPrefs.Save();
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
            PlayerPrefs.SetInt(QUALITY, value);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(value);
            qualityLevel = value;
        }
    }
    private int qualityLevel;
}
