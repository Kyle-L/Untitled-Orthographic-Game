using NPBehave;
using UnityEngine;
using static NPBehave.Action;

public abstract class Controller : MonoBehaviour {

    [Header("Character")]
    public Transform character;
    private Vector3 offsetPos;
    private Vector3 offsetRot;

    [Header("Wandering")]
    public Transform[] wanderingPoints;
    public float wanderingTime = 5;
    public float wanderingTimeDeviation = 2;

    [Header("Posing")]
    public string poseAnimationLayerName = "Base Layer";
    private int poseAnimationIndex;

    public enum States {
        Idle,
        Wandering,
        Interacting,
        Posed,
        UserControlled
    }

    [Header("Behavior Tree")]
    protected Root behaviorTree;
    protected Blackboard blackboard;
    public enum BlackBoardVars {
        Look,
        State,
        NewInteractingObject,
        InteractingObject,
        InteractingObjectType,
        Destination
    }

    public bool isAlive = true;

    // Components
    public MovementController MovementController { get; protected set; }
    public PlayerObjectHolder PlayerObjectHolder { get; protected set; }
    protected Animator _animator;

    protected void Start() {
        MovementController = GetComponent<MovementController>();
        PlayerObjectHolder = GetComponent<PlayerObjectHolder>();
        _animator = GetComponent<Animator>();

        // Gets the pose animation layer.
        poseAnimationIndex = _animator.GetLayerIndex(poseAnimationLayerName);

        // Gets how much the character is offset.
        offsetPos = character.localPosition;
        offsetRot = character.localRotation.eulerAngles;

        // create our behaviour tree and get it's blackboard
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;

        // attach the debugger component if executed in editor (helps to debug in the inspector) 
#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif

        // start the behaviour tree
        behaviorTree.Start();
    }

    protected Root CreateBehaviourTree() {
        // We always need a root node
        return new Root(

            // Kick up our service to update npc behavior and the Blackboard values every 500 milliseconds
            new Service(0.500f, UpdateBlackBoards,

                // Run the nodes in parallel and succeed so long as one succeeds.
                new Parallel(Parallel.Policy.ONE, Parallel.Policy.ALL,

                    // Tells the npc to look at a random lookable object so long as the look param is true.
                    new BlackboardCondition("Look", Operator.IS_EQUAL, true, Stops.BOTH,
                        new Action((bool success) => {
                            if (!success) {
                                MovementController.LookAtRandom(CharacterSerializer.instance.Lookable);
                                return Action.Result.PROGRESS;
                            } else {
                                return Action.Result.FAILED;
                            }
                        })
                    ),

                    // Run the node dictated by the state param. Only one needs to run for the node to be successful.
                    new Selector(

                        // If the state is user controlled, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.UserControlled, Stops.IMMEDIATE_RESTART,
                            // In the user controlled state, the npc does nothing.
                            new WaitUntilStopped()
                        ) { Label = "User Controlled" },

                        // If the state is idle, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Idle, Stops.IMMEDIATE_RESTART,
                            // In the idle state, the npc does nothing.
                            new WaitUntilStopped()
                        ) { Label = "Idle" },

                        // If the state is posed, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Posed, Stops.IMMEDIATE_RESTART,
                            // In the posed state, the npc does nothing.
                            new WaitUntilStopped()
                        ) { Label = "Posed" },

                        // If the state is wandering, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Wandering, Stops.IMMEDIATE_RESTART,
                            // Run a sequence node that tells the player to move to a position, then wait.
                            new Sequence(
                                // A multi frame action that tells the player to go to a random location.
                                new Action(() => {
                                    blackboard[BlackBoardVars.Destination.ToString()] = wanderingPoints[UnityEngine.Random.Range(0, wanderingPoints.Length)].transform.position;
                                }),
                                new NavMoveTo(this, BlackBoardVars.Destination.ToString()),
                                // Now at a wandering point, the npc waits to wander again.
                                new Wait(wanderingTime, wanderingTimeDeviation)
                            )
                        ) { Label = "Wander" },

                        // If the state is interacting, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Interacting, Stops.IMMEDIATE_RESTART,
                            new Repeater(
                                new Succeeder(
                                    new Sequence(
                                    // Determines the type of object that the npc is interacting with.
                                    new Action(() => {
                                        blackboard[BlackBoardVars.InteractingObject.ToString()] = blackboard[BlackBoardVars.NewInteractingObject.ToString()];
                                        if (blackboard[BlackBoardVars.InteractingObject.ToString()].GetType().BaseType == typeof(MonoBehaviour) || blackboard[BlackBoardVars.InteractingObject.ToString()].GetType().BaseType == typeof(InteractBase)) {
                                            blackboard[BlackBoardVars.InteractingObjectType.ToString()] = blackboard[BlackBoardVars.InteractingObject.ToString()].GetType();
                                        } else {
                                            blackboard[BlackBoardVars.InteractingObjectType.ToString()] = blackboard[BlackBoardVars.InteractingObject.ToString()].GetType().BaseType;
                                        }
                                    }),
                                    new Selector(
                                        // If the interacting object param has already been set, run this node.
                                        new BlackboardCondition(BlackBoardVars.InteractingObjectType.ToString(), Operator.IS_EQUAL, typeof(Controller), Stops.BOTH,
                                            // This sequence represents the actions taken to talk.
                                            new Sequence(
                                                // First, the npc will move to the front of the controller.
                                                new Action(() => {
                                                    Controller talkingTo = blackboard.Get<Controller>(BlackBoardVars.InteractingObject.ToString());
                                                    blackboard[BlackBoardVars.Destination.ToString()] = talkingTo.transform;
                                                }),
                                                new NavMoveTo(this, BlackBoardVars.Destination.ToString()),
                                                // Then, the npc will face the other controller.
                                                new Action(() => {
                                                    Controller talkingTo = blackboard.Get<Controller>(BlackBoardVars.InteractingObject.ToString());
                                                    MovementController.Face(talkingTo.transform);
                                                }),
                                                // Finally, the npc will wait in the state till something changes.
                                                new WaitUntilStopped()
                                            )
                                        ) { Label = "Talking" },

                                        new BlackboardCondition(BlackBoardVars.InteractingObjectType.ToString(), Operator.IS_EQUAL, typeof(Interactable), Stops.BOTH,
                                            // This sequence represents the actions taken to talk.
                                            new Sequence(
                                                new Action(() => {
                                                    Interactable talkingTo = blackboard.Get<Interactable>(BlackBoardVars.InteractingObject.ToString());
                                                    blackboard[BlackBoardVars.Destination.ToString()] = talkingTo.interactionPoint;
                                                }),
                                                // First, the npc will move to the front of the controller.
                                                new NavMoveTo(this, BlackBoardVars.Destination.ToString()),
                                                // Then, the npc will face the other controller.
                                                new Action((Request result) => {
                                                    Interactable interactionObject = blackboard.Get<Interactable>(BlackBoardVars.InteractingObject.ToString());
                                                    if (result == Request.CANCEL) {
                                                        MovementController.SetCharacterControllerState(true);
                                                        MovementController.TriggerAnimation(interactionObject.animationStopString);
                                                        interactionObject.Stop(this);
                                                        return Action.Result.SUCCESS;
                                                    } else if (result == Request.START) {
                                                        MovementController.AlignPosition(interactionObject.interactionPoint);
                                                        MovementController.AlignRotation(interactionObject.interactionPoint);

                                                        MovementController.TriggerAnimation(interactionObject.animationGoString);
                                                        MovementController.SetCharacterControllerState(false);

                                                        interactionObject.Go(this);
                                                        return Action.Result.PROGRESS;
                                                    } else {
                                                        return Action.Result.PROGRESS;
                                                    }
                                                })
                                            )
                                        ) { Label = "Interactable" },

                                        new BlackboardCondition(BlackBoardVars.InteractingObjectType.ToString(), Operator.IS_EQUAL, typeof(Viewable), Stops.LOWER_PRIORITY,
                                            // This sequence represents the actions taken to talk.
                                            new Sequence(
                                                new Action(() => {
                                                    Viewable talkingTo = blackboard.Get<Viewable>(BlackBoardVars.InteractingObject.ToString());
                                                    blackboard[BlackBoardVars.Destination.ToString()] = talkingTo.interactionPoint;
                                                }),
                                                // First, the npc will move to the front of the controller.
                                                new NavMoveTo(this, BlackBoardVars.Destination.ToString()),

                                                new Action(() => {
                                                    Viewable talkingTo = blackboard.Get<Viewable>(BlackBoardVars.InteractingObject.ToString());
                                                    talkingTo.Go();
                                                    MovementController.Face(talkingTo.transform);
                                                    SetState(States.UserControlled);
                                                })
                                            )
                                        ) { Label = "Viewable" },

                                        new BlackboardCondition(BlackBoardVars.InteractingObjectType.ToString(), Operator.IS_EQUAL, typeof(Pickupable), Stops.LOWER_PRIORITY,
                                            // This sequence represents the actions taken to talk.
                                            new Sequence(
                                                new Action(() => {
                                                    Pickupable talkingTo = blackboard.Get<Pickupable>(BlackBoardVars.InteractingObject.ToString());
                                                    blackboard[BlackBoardVars.Destination.ToString()] = talkingTo.transform;
                                                }),
                                                // First, the npc will move to the front of the controller.
                                                new NavMoveTo(this, BlackBoardVars.Destination.ToString()),

                                                new Action(() => {
                                                    Pickupable talkingTo = blackboard.Get<Pickupable>(BlackBoardVars.InteractingObject.ToString());
                                                    PlayerObjectHolder.SetPickup(talkingTo);
                                                    SetState(States.UserControlled);
                                                })
                                            )
                                        ) { Label = "Pickupable" }
                                    )
                                )
                                )
                            )
                        ),
                        new WaitUntilStopped()
                    )
                )
            )
        );
    }

    public void Pose(string animation, Transform pos) {
        SetState(States.Posed);
        MovementController.Pose(poseAnimationIndex, animation, true, false, true);
        MovementController.SetPosition(pos, false);
    }

    public void InteractWith(Object go) {
        if (blackboard.Get<Object>(BlackBoardVars.InteractingObject.ToString()) == go && GetState() != States.UserControlled) {
            return;
        }

        ModifyBlackBoard(BlackBoardVars.NewInteractingObject, go);
        ModifyBlackBoard(BlackBoardVars.State, States.Interacting);
        ModifyBlackBoard(BlackBoardVars.InteractingObjectType);
    }

    public void SetState(States state) {
        ModifyBlackBoard(BlackBoardVars.State, state);
    }

    public States GetState() {
        return blackboard.Get<States>(BlackBoardVars.State.ToString());
    }

    /// <summary>
    /// Modifies a blackboard variable to be unset.
    /// </summary>
    /// <param name="key"></param>
    private void ModifyBlackBoard(BlackBoardVars key) {
        blackboard.Unset(key.ToString());
    }

    /// <summary>
    /// Modifies a blackboard variable.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void ModifyBlackBoard(BlackBoardVars key, object value) {
        blackboard[key.ToString()] = value;
    }

    public void UpdateBlackBoards() {
        blackboard[BlackBoardVars.Look.ToString()] = true;
    }

    public void Die() {
        isAlive = false;
        //MovementController._navMeshAgent.enabled = false;
        MovementController.SetCharacterControllerState(false);
        MovementController.RagDoll();
    }

    public void Live() {


        transform.rotation = character.rotation;
        character.rotation = Quaternion.Euler(transform.rotation.eulerAngles + offsetRot);

        MovementController._navMeshAgent.enabled = true;

        MovementController.SetCharacterControllerState(true);

        MovementController.SetPosition(character);
        character.localPosition = offsetPos;

        isAlive = true;
        MovementController.StopRagdoll();
        // Need a way to set the body position

    }

}
