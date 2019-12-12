﻿using System.Collections;
using UnityEngine;
/// <summary>
/// The overall controller for an npc.
/// </summary>
public class NPCController : MonoBehaviour {

    [Header("States")]
    [SerializeField]
    private States startState = States.Idle;

    public States CurrentState {
        get { return currentState; }
        set {
            currentState = value;
            UpdateState();
        }
    }
    private States currentState;

    public enum States {
        Idle,
        Wandering,
        Interacting,
        Talking,
        Attacking,
        Searching,
        Dying
    }

    [Header("Wandering")]
    [SerializeField]
    private Transform[] wanderingPoints;
    [SerializeField]
    private float wanderingTime = 10;
    [SerializeField]
    private float wanderingTimeDeviation = 10;

    [Header("Interacting")]
    [SerializeField]
    private Transform[] interactingPoints;

    // Components
    [SerializeField]
    private NPCDialogueController _npcDialogueController;
    public NPCDialogueController NPCDialogueController {
        get {
            return _npcDialogueController;
        }
    }

    [SerializeField]
    private NPCMovementController _npcMovementController;
    public NPCMovementController NPCMovementController {
        get {
            return _npcMovementController;
        }
    }

    private void UpdateState() {
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

    private void Start() {
        // Add WanderingReached to the ReachedDestination event.
        NPCMovementController.ReachedDestination += (sender, args) => { WanderReached(); };

        /* Updates the current state if the current
           state has any actions that need to be run
           in Start. */
        CurrentState = startState;
    }

    private void Update() {

    }

    /// <summary>
    /// Stops the NPC from doing other actions.
    /// </summary>
    public void Idle() {
        NPCMovementController.Stop();
    }

    #region Wander
    /// <summary>
    /// Indicates to the NPC to start wandering.
    /// </summary>
    public void Wander() {
        // If not in the wandering state, stop.
        if (currentState != States.Wandering) {
            return;
        }
        // Set the first location to visit.
        NPCMovementController.SetLocation(wanderingPoints[Random.Range(0, wanderingPoints.Length)]);
    }

    /// <summary>
    /// How the NPC should react when a destination is reached while in
    /// the wandering state.
    /// </summary>
    private void WanderReached() {
        if (wandering != null) {
            StopCoroutine(wandering);
        }

        wandering = StartCoroutine(WanderingWait(
           wanderingTime + Random.Range(-wanderingTimeDeviation, wanderingTimeDeviation),
           wanderingPoints[Random.Range(0, wanderingPoints.Length)]));
    }

    /* The coroutine for the wandering wait.
       This ensures there is only one coroutine
       instance. */
    Coroutine wandering;

    /// <summary>
    /// Sets the next locations once the amount of time alloted
    /// passes.
    /// </summary>
    /// <param name="time">The amount of time to wait.</param>
    /// <param name="loc">The location to visit after waiting.</param>
    /// <returns></returns>
    private IEnumerator WanderingWait(float time, Transform loc) {
        yield return new WaitForSeconds(time);
        NPCMovementController.SetLocation(loc);
    }
    #endregion

    #region

    public void Talk() {

    }

    #endregion


    public void Interact() {

    }

    public void Attack() {

    }

    public void Search() {

    }

    public void Die() {

    }

}
