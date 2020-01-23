using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    [Header("States")]
    [SerializeField]
    protected States startState = States.Idle;

    public States currentState { get; protected set; }
    protected Stack<States> stateHistory;

    [Header("Transforms")]
    public Transform head;

    protected Coroutine stateCoroutine;

    public enum States {
        Idle,
        Wandering,
        Interacting,
        Talking,
        Searching,
        Dying
    }

    protected MovementController _movementController;

    protected void Start() {
        stateHistory = new Stack<States>();

        _movementController = GetComponent<MovementController>();
    }

    public void Die() {
        _movementController.Stop();
        _movementController.RagDoll();
    }

    public void Live() {
        _movementController.Stop();
        _movementController.StopRagdoll();
    }

}
