using UnityEngine;

public class PlayerControllerMain : MonoBehaviour {
    public static PlayerControllerMain instance;

    //Controls
    private bool canControl = true;
    public bool Control {
        get {
            return canControl;
        }
        set {
            canControl = value;
            _playerMovementController.Control = !isPaused && canControl;
        }
    }

    private bool isPaused = false;
    public bool Pause {
        get {
            return isPaused;
        }
        set {
            isPaused = value;
            _playerMovementController.Control = !isPaused && canControl;
        }
    }

    [SerializeField]
    private PlayerMovementController2 _playerMovementController;
    [SerializeField]
    private PlayerSettingsController _playerSettingsController;

    public void Start() {
        instance = this;

        _playerMovementController = GetComponent<PlayerMovementController2>();
        _playerSettingsController = GetComponent<PlayerSettingsController>();
    }

}
