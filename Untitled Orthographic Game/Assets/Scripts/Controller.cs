using System.Collections.Generic;
using UnityEngine;
using NPBehave;

public abstract class Controller : MonoBehaviour {

    [Header("States")]
    public States currentState = States.Idle;

    public enum States {
        Idle,
        Wandering,
        Interacting,
        Posed
    }
    protected Stack<States> stateHistory;

    // Status
    public bool isAlive = true;

    [Header("Behavior Tree")]
    protected Root behaviorTree;
    protected Blackboard blackboard;

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

    protected abstract Root CreateBehaviourTree();

    protected abstract void UpdateBlackBoards();

    public void Die() {
        MovementController.Stop();
        MovementController.RagDoll();
    }

    public void Live() {
        MovementController.Stop();
        MovementController.StopRagdoll();
    }

}
