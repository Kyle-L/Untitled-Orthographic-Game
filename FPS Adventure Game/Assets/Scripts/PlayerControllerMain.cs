using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerSettingsController))]
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

    public PlayerMovementController _playerMovementController { get; private set; }
    public PlayerSettingsController _playerSettingsController { get; private set; }

    public void Start() {
        instance = this;

        _playerMovementController = GetComponent<PlayerMovementController>();
        _playerSettingsController = GetComponent<PlayerSettingsController>();
    }

}
