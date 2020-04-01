using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerInteractionController))]
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
    public PlayerInteractionController PlayerDialogueController { get; private set; }
    public PlayerSettingsController PlayerSettingsController { get; private set; }

    public new void Start() {
        base.Start();
        instance = this;

        PlayerMovementController = GetComponent<PlayerMovementController>();
        PlayerDialogueController = GetComponent<PlayerInteractionController>();
        PlayerSettingsController = GetComponent<PlayerSettingsController>();
    }

}
