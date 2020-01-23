using System.Collections;
using UnityEngine;
using NPBehave;

/// <summary>
/// The overall controller for an npc.
/// </summary>
public class NPCController : Controller {

    [Header("Wandering")]
    public Transform[] wanderingPoints;
    public float wanderingTime = 10;
    public float wanderingTimeDeviation = 10;

    [Header("Interacting")]
    public Transform[] interactingPoints;

    public NPCDialogueController NPCDialogueController { get; private set; }

    public NPCMovementController NPCMovementController { get; private set; }

    private new void Start() {
        // Get componenets
        NPCDialogueController = GetComponent<NPCDialogueController>();
        NPCMovementController = GetComponent<NPCMovementController>();

        base.Start();

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

    #region Wander
    /// <summary>
    /// Indicates to the NPC to start wandering.
    /// </summary>
    public void Wander() {
        // Set the first location to visit.
        if (!NPCMovementController.isWalking) {
            NPCMovementController.SetLocation(wanderingPoints[UnityEngine.Random.Range(0, wanderingPoints.Length)]);
        }
    }

    #endregion

    #region Talk

    public void Talk(GameObject gameObject) {
        NPCMovementController.Stop();
        NPCMovementController.SetLocation(gameObject.transform.position + gameObject.transform.forward);
        //NPCMovementController.LookAt(gameObject.transform.position);
        stateCoroutine = StartCoroutine(Talking(gameObject));
    }

    IEnumerator Talking(GameObject gameObject) {
        while (NPCMovementController.isWalking) {
            yield return null;
        }
        NPCMovementController.Face(gameObject.transform);
    }

    public void StopTalk() {
        if (stateCoroutine != null) {
            StopCoroutine(stateCoroutine);
        }
        NPCMovementController.StopFace();
    }

    #endregion

    private Root behaviorTree;
    private Blackboard blackboard;

    private Root CreateBehaviourTree() {
        // we always need a root node
        return new Root(

            // kick up our service to update the "playerDistance" and "playerLocalPos" Blackboard values every 125 milliseconds
            new Service(0.125f, UpdateBlackBoards,

                new Selector(

                    new BlackboardCondition("state", Operator.IS_EQUAL, States.Idle, Stops.IMMEDIATE_RESTART,

                        new Action(() => _movementController.Stop()) { Label = "Idle" }
                    
                    ),

                    new BlackboardCondition("state", Operator.IS_EQUAL, States.Wandering, Stops.IMMEDIATE_RESTART,

                        new Sequence(

                            new Action(() => Wander()) { Label = "Wander" }

                        )

                    )

                )
            )
        );
    }

    public void UpdateBlackBoards () {
        blackboard["state"] = startState;
    }

}
