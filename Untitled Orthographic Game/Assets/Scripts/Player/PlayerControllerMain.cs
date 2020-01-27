using NPBehave;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerDialogueController))]
[RequireComponent(typeof(PlayerObjectHolder))]
[RequireComponent(typeof(PlayerSettingsController))]
public class PlayerControllerMain : Controller {
    public static PlayerControllerMain instance;

    // Controls
    private bool canControl = true;
    public bool Control {
        get {
            return canControl;
        }
        set {
            canControl = value;
            PlayerMovementController.Control = !isPaused && canControl;
        }
    }

    private bool isPaused = false;
    public bool Pause {
        get {
            return isPaused;
        }
        set {
            isPaused = value;
            PlayerMovementController.Control = !isPaused && canControl;
        }
    }

    // Components
    public PlayerMovementController PlayerMovementController { get; private set; }
    public PlayerDialogueController PlayerDialogueController { get; private set; }
    public PlayerObjectHolder PlayerObjectHolder { get; private set; }
    public PlayerSettingsController PlayerSettingsController { get; private set; }

    public new void Start() {
        base.Start();
        instance = this;

        PlayerMovementController = GetComponent<PlayerMovementController>();
        PlayerDialogueController = GetComponent<PlayerDialogueController>();
        PlayerObjectHolder = GetComponent<PlayerObjectHolder>();
        PlayerSettingsController = GetComponent<PlayerSettingsController>();
    }

}
