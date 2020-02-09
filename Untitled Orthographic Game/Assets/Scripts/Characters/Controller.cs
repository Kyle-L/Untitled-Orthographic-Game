using NPBehave;
using System.Collections.Generic;
using UnityEngine;
using static NPBehave.Action;

public abstract class Controller : MonoBehaviour {

    [Header("Wandering")]
    public Transform[] wanderingPoints;
    public float wanderingTime = 5;
    public float wanderingTimeDeviation = 2;

    public enum States {
        Idle,
        Wandering,
        Interacting,
        Posed,
        UserControlled
    }
    protected Stack<States> stateHistory;

    [Header("Behavior Tree")]
    protected Root behaviorTree;
    protected Blackboard blackboard;
    public enum BlackBoardVars {
        Look,
        State,
        InteractingObject,
        InteractingObjectType,
        Destination
    }

    [Header("Traits")]
    public List<Traits> activeTraits;
    public enum Traits { Talkative, Curious, EasilyBored, Lazy, }

    // Status
    public bool isAlive = true;

    // Components
    public MovementController MovementController { get; protected set; }

    protected void Start() {
        stateHistory = new Stack<States>();

        MovementController = GetComponent<MovementController>();

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
                                new NavMoveTo(MovementController._navMeshAgent, BlackBoardVars.Destination.ToString()),
                                // Now at a wandering point, the npc waits to wander again.
                                new Wait(wanderingTime, wanderingTimeDeviation)
                            )
                        ) { Label = "Wander" },

                        // If the state is interacting, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Interacting, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                // Determines the type of object that the npc is interacting with.
                                new Action(() => {
                                    blackboard[BlackBoardVars.InteractingObjectType.ToString()] = blackboard[BlackBoardVars.InteractingObject.ToString()].GetType().BaseType;
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
                                            new NavMoveTo(MovementController._navMeshAgent, BlackBoardVars.Destination.ToString()),
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
                                            new NavMoveTo(MovementController._navMeshAgent, BlackBoardVars.Destination.ToString()),
                                            // Then, the npc will face the other controller.
                                            new Action((Request result) => {
                                                if (result == Request.CANCEL) {
                                                    MovementController.SetCharacterControllerState(true);
                                                    MovementController.TriggerAnimation(MovementController.AnimationTriggers.Exit);
                                                    return Action.Result.SUCCESS;
                                                } else if (result == Request.START) {
                                                    Interactable interactionObject = blackboard.Get<Interactable>(BlackBoardVars.InteractingObject.ToString());
                                                    MovementController.AlignPosition(interactionObject.interactionPoint);
                                                    MovementController.AlignRotation(interactionObject.interactionPoint);
                                                    MovementController.TriggerAnimation(MovementController.AnimationTriggers.SitDown);
                                                    MovementController.SetCharacterControllerState(false);
                                                    return Action.Result.PROGRESS;
                                                } else {
                                                    return Action.Result.PROGRESS;
                                                }
                                            })
                                        )
                                    ) { Label = "Interactable" }
                                )
                            )
                        ),
                        new WaitUntilStopped()
                    )
                )
            )
        );
    }

    public void InteractWith (Interactable go) {
        ModifyBlackBoard(BlackBoardVars.InteractingObject, go);
        ModifyBlackBoard(BlackBoardVars.State, States.Interacting);
    }

    public void InteractWith(Controller go) {
        ModifyBlackBoard(BlackBoardVars.InteractingObject, go);
        ModifyBlackBoard(BlackBoardVars.State, States.Interacting);
    }

    public void SetState (States state) {
        ModifyBlackBoard(BlackBoardVars.State, state);
    }

    /// <summary>
    /// Modifies a blackboard variable.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void ModifyBlackBoard(BlackBoardVars key, object value) {
        blackboard[key.ToString()] = value;
    }

    /// <summary>
    /// Modifies a blackboard variable.
    /// Precondition: The length of key is equal to the length of value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void ModifyBlackBoard(BlackBoardVars[] key, object[] value) {
        if (key.Length != value.Length) {
            return;
        }

        for (int index = 0; index < key.Length; index++) {
            blackboard[key[index].ToString()] = value[index];
        }
    }

    public void UpdateBlackBoards() {
        blackboard[BlackBoardVars.Look.ToString()] = true;
    }

    public void Die() {
        MovementController._navMeshAgent.enabled = false;
        MovementController.RagDoll();
    }

    public void Live() {
        MovementController._navMeshAgent.enabled = true;
        MovementController.StopRagdoll();
    }

}
