using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerDialogueController))]
[RequireComponent(typeof(PlayerObjectHolder))]
[RequireComponent(typeof(PlayerSettingsController))]
public class PlayerControllerMain : Controller {
    public static PlayerControllerMain instance;

    //Controls
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

    public override void Idle() {
        //throw new System.NotImplementedException();
    }

    public override void Wander() {
        //throw new System.NotImplementedException();
    }

    public override void Talk() {
        //throw new System.NotImplementedException();
    }

    public override void Interact() {
        //throw new System.NotImplementedException();
    }

    public override void Attack() {
        //throw new System.NotImplementedException();
    }

    public override void Search() {
        //throw new System.NotImplementedException();
    }

    public override void Die() {
        //throw new System.NotImplementedException();
    }

    public override void Live() {
        //throw new System.NotImplementedException();
    }
}
