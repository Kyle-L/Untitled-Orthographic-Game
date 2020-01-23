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
        Attacking,
        Searching,
        Dying
    }

    protected MovementController _movementController;

    protected void Start() {
        stateHistory = new Stack<States>();

        /* Updates the current state if the current
           state has any actions that need to be run
           in Start. */
        UpdateState(startState);

        _movementController = GetComponent<MovementController>();
    }

    protected void Update() {
        switch (currentState) {
            case States.Idle:
                _movementController.LookAtRandom(CharacterSerializer.instance.Lookable);
                break;
            case States.Wandering:
                _movementController.LookAtRandom(CharacterSerializer.instance.Lookable);
                break;
            case States.Talking:
                break;
            case States.Interacting:
                break;
            case States.Attacking:
                break;
            case States.Searching:
                break;
            case States.Dying:
                break;
        }
    }

    #region State management
    /// <summary>
    /// Updates the current state of the character and adds the old state to the state history.
    /// </summary>
    /// <param name="state"></param>
    protected void UpdateState(States state) {
        // Add the current state to the history.
        stateHistory.Push(currentState);
        // Quit the state coroutine if it is running.
        if (stateCoroutine != null) {
            StopCoroutine(stateCoroutine);
        }
        // Update the current state.
        currentState = state;
        // Refresh the state.
        RefreshState();
    }

    /// <summary>
    /// Updates the current state of the character to the previous state of the character.
    /// If the history has no states in it, the state of the character's state won't be updated.
    /// </summary>
    protected void UpdateToLastState() {
        // Quit if the history is empty.
        if (stateHistory.Count == 0) {
            return;
        }
        // Quit the state coroutine if it is running.
        if (stateCoroutine != null) {
            StopCoroutine(stateCoroutine);
        }
        // Set the current state to the last state.
        currentState = stateHistory.Pop();
        // Refresh the state.
        RefreshState();
    }

    /// <summary>
    /// Refreshes the state and performs corresponding methods.
    /// </summary>
    protected void RefreshState() {
        switch (currentState) {
            case States.Idle:
                Idle();
                break;
            case States.Wandering:
                Wander();
                break;
            case States.Talking:
                Talk();
                break;
            case States.Interacting:
                break;
            case States.Attacking:
                break;
            case States.Searching:
                break;
            case States.Dying:
                break;
        }
    }
    #endregion

    public abstract void Idle();

    public abstract void Wander();

    public abstract void Talk();

    public abstract void Interact();

    public abstract void Attack();

    public abstract void Search();

    public void Die() {
        _movementController.Stop();
        _movementController.RagDoll();
    }

    public void Live() {
        _movementController.Stop();
        _movementController.StopRagdoll();
    }

}
