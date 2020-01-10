using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// The overall controller for an npc.
/// </summary>
public class NPCController : MonoBehaviour {

    [Header("States")]
    [SerializeField]
    private States startState = States.Idle;

    public States currentState { get; private set; }
    private Stack<States> stateHistory;

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

    [Header("Transforms")]
    public Transform head;

    // Components
    private NPCDialogueController _npcDialogueController;
    public NPCDialogueController NPCDialogueController {
        get {
            return _npcDialogueController;
        }
    }

    private NPCMovementController _npcMovementController;
    public NPCMovementController NPCMovementController {
        get {
            return _npcMovementController;
        }
    }

    List<Transform> allParticipants;

    private void Update() {
        switch (currentState) {
            case States.Idle:
                _npcMovementController.LookAtRandom(allParticipants.ToArray());
                break;
            case States.Wandering:
                //Transform shortest = null;
                //foreach (GameObject gameObject in allParticipants) {
                //    if (shortest == null || Vector3.Distance(head.position, gameObject.transform.position) < Vector3.Distance(head.position, shortest.transform.position)) {
                //        if (!gameObject.transform.Equals(head.transform)) {
                //            shortest = gameObject.transform;
                //        }
                //    }
                //}
                _npcMovementController.LookAtRandom(allParticipants.ToArray());
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

    private void Start() {
        // Get componenets
        _npcDialogueController = GetComponent<NPCDialogueController>();
        _npcMovementController = GetComponent<NPCMovementController>();

        // Add WanderingReached to the ReachedDestination event.
        NPCMovementController.ReachedDestination += (sender, args) => { WanderReached(); };

        stateHistory = new Stack<States>();

        /* Updates the current state if the current
           state has any actions that need to be run
           in Start. */
        UpdateState(startState);

        //allParticipants = new List<GameObject>(GameObject.FindGameObjectsWithTag("Lookable"));
        allParticipants = new List<Transform>();
        allParticipants.AddRange(GameObject.FindGameObjectsWithTag("Lookable").Select(go => go.transform));
        allParticipants.Remove(head.transform);
    }


    #region State management
    private void UpdateState(States state) {
        stateHistory.Push(currentState);
        currentState = state;
        RefreshState();
    }

    private void UpdateToLastState() {
        if (stateHistory.Count == 0) {
            return;
        }
        currentState = stateHistory.Pop();
        RefreshState();
    }

    private void RefreshState() {
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

    #region Idle
    /// <summary>
    /// Stops the NPC from doing other actions.
    /// </summary>
    public void Idle() {
        NPCMovementController.Stop();
    }
    #endregion

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
           wanderingTime + Random.Range(-wanderingTimeDeviation, wanderingTimeDeviation)));
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
    private IEnumerator WanderingWait(float time) {
        yield return new WaitForSeconds(time);
        Wander();
    }
    #endregion

    #region Talk

    public void Talk() {

    }

    public void Talk(GameObject gameObject) {
        UpdateState(States.Talking);
        NPCMovementController.Stop();
        NPCMovementController.SetLocation(gameObject.transform.position + gameObject.transform.forward);
        //NPCMovementController.LookAt(gameObject.transform.position);
        StartCoroutine(Talking(gameObject));
    }

    Coroutine talk;

    IEnumerator Talking (GameObject gameObject) {
        while (NPCMovementController.isWalking) {
            yield return null;
        }
        NPCMovementController.Face(gameObject.transform);
    }

    public void StopTalk() {
        UpdateToLastState();
        if (talk != null) {
            StopCoroutine(talk);
        }
        NPCMovementController.StopFace();
    }

    #endregion


    public void Interact() {

    }

    public void Attack() {

    }

    public void Search() {

    }

    public void Die() {
        UpdateState(States.Talking);
        NPCMovementController.Stop();
        NPCMovementController.RagDoll();
    }

    public void Live() {
        UpdateState(States.Talking);
        NPCMovementController.Stop();
        NPCMovementController.StopRagdoll();
    }

    Transform GetClosestEnemy(Transform[] npcs) {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in npcs) {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }

}
