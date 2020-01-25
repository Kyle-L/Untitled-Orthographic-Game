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

    protected override Root CreateBehaviourTree() {
        // we always need a root node
        return new Root(

            // kick up our service to update the "playerDistance" and "playerLocalPos" Blackboard values every 125 milliseconds
            new Service(0.5f, UpdateBlackBoards,

                new Parallel(Parallel.Policy.ONE, Parallel.Policy.ALL,

                    new BlackboardCondition("look", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                        new Action((bool success) => {
                            if (!success) {
                                MovementController.LookAtRandom(CharacterSerializer.instance.Lookable);
                                return Action.Result.PROGRESS;
                            } else {
                                MovementController.StopLookAt();
                                return Action.Result.FAILED;
                            }
                        })
                    )
                )
            )
        );
    }

    protected override void UpdateBlackBoards() {
        blackboard["look"] = true;
        blackboard["state"] = currentState;
    }
}
