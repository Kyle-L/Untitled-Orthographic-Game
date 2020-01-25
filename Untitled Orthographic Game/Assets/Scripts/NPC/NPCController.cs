using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The overall controller for an npc.
/// </summary>
public class NPCController : Controller {

    [Header("Wandering")]
    public Transform[] wanderingPoints;
    public float wanderingTime = 5;
    public float wanderingTimeDeviation = 2;

    public enum Traits { Talkative, Curious, Bored }
    public List<Traits> activeTraits;

    // Components
    public NPCDialogueController NPCDialogueController { get; private set; }
    public NPCMovementController NPCMovementController { get; private set; }

    protected new void Start() {
        // Get componenets
        NPCDialogueController = GetComponent<NPCDialogueController>();
        NPCMovementController = GetComponent<NPCMovementController>();

        base.Start();
    }

    protected override Root CreateBehaviourTree() {
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
                                MovementController.StopLookAt();
                                return Action.Result.FAILED;
                            }
                        })
                    ),

                    // Run the node dictated by the state param. Only one needs to run for the node to be successful.
                    new Selector(

                        // If the state is idle, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Idle, Stops.BOTH,
                            // In the idle state, the npc does nothing.
                            new WaitUntilStopped() { Label = "Idle" }
                        ),

                        // If the state is posed, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Posed, Stops.BOTH,
                            // In the posed state, the npc does nothing.
                            new WaitUntilStopped() { Label = "Posed" }
                        ),

                        // If the state is wandering, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Wandering, Stops.BOTH,
                            // Run a sequence node that tells the player to move to a position, then wait.
                            new Sequence(
                                // A multi frame action that tells the player to go to a random location.
                                new Action((Action.Request req) => {
                                    // On start, a random location is chosen.
                                    if (req == Action.Request.START) {
                                        NPCMovementController.SetLocation(wanderingPoints[UnityEngine.Random.Range(0, wanderingPoints.Length)]);
                                        return Action.Result.PROGRESS;
                                    // If the player is walking, the action is still in progress.
                                    } else if (MovementController.isWalking) {
                                        return Action.Result.PROGRESS;
                                    // If the request indicates the action is canceled, stop movement and fail the node.
                                    } else if (req == Action.Request.CANCEL) {
                                        MovementController.Stop();
                                        return Action.Result.FAILED;
                                    // If the action isn't canceled and the player isn't walking, the action is successful.
                                    } else {
                                        return Action.Result.SUCCESS;
                                    }
                                }) { Label = "Wander" },
                                // Now at a wandering point, the npc waits to wander again.
                                new Wait(wanderingTime, wanderingTimeDeviation)
                            )
                        ),
                        
                        // If the state is interacting, run this.
                        new BlackboardCondition(BlackBoardVars.State.ToString(), Operator.IS_EQUAL, States.Interacting, Stops.IMMEDIATE_RESTART,
                            new Selector(
                                // If the interacting object param has already been set, run this node.
                                new BlackboardCondition(BlackBoardVars.interactingObjectType.ToString(), Operator.IS_EQUAL, typeof(PlayerControllerMain), Stops.BOTH,
                                    // This sequence represents the actions taken to talk.
                                    new Sequence(
                                        // First, the npc will move to the front of the controller.
                                        new Action((Action.Request req) => {
                                            Controller talkingTo = blackboard.Get<PlayerControllerMain>(BlackBoardVars.InteractingObject.ToString());
                                            if (req == Action.Request.START) {
                                                MovementController.SetLocation(talkingTo.transform.position + talkingTo.transform.forward);
                                                return Action.Result.PROGRESS;
                                            } else if (MovementController.isWalking) {
                                                return Action.Result.PROGRESS;
                                            } else if (req == Action.Request.CANCEL) {
                                                MovementController.Stop();
                                                return Action.Result.FAILED;
                                            } else {
                                                return Action.Result.SUCCESS;
                                            }
                                        }),
                                        // Then, the npc will face the other controller.
                                        new Action(() => {
                                            Controller talkingTo = blackboard.Get<Controller>(BlackBoardVars.InteractingObject.ToString());
                                            MovementController.Face(talkingTo.transform);
                                        }),
                                        // Finally, the npc will wait in the state till something changes.
                                        new WaitUntilStopped()
                                    ) { Label = "Talking"}
                                ),

                                new WaitUntilStopped()

                            )
                        )
                    )
                )
            )
        );
    }

    /// <summary>
    /// Modifies a blackboard variable.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void ModifyBlackBoard (BlackBoardVars key, object value) {
        blackboard[key.ToString()] = value;
    }

    /// <summary>
    /// Modifies a blackboard variable.
    /// Precondition: The length of key is equal to the length of value.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void ModifyBlackBoard(BlackBoardVars[] key, object[] value) {
        if (key.Length != value.Length) {
            return;
        }

        for (int index = 0; index < key.Length; index++) {
            blackboard[key[index].ToString()] = value[index];
        }
    }

    public enum BlackBoardVars { 
        Look, 
        State, 
        InteractingObject, 
        interactingObjectType 
    }

    protected override void UpdateBlackBoards() {
        blackboard[BlackBoardVars.Look.ToString()] = true;
        blackboard[BlackBoardVars.State.ToString()] = currentState;
        if (blackboard[BlackBoardVars.InteractingObject.ToString()] != null) {
            blackboard[BlackBoardVars.interactingObjectType.ToString()] = blackboard[BlackBoardVars.InteractingObject.ToString()].GetType();
        }
    }

}
