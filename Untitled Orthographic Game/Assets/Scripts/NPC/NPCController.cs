using System.Collections;
using UnityEngine;
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

        // Add WanderingReached to the ReachedDestination event.
        NPCMovementController.ReachedDestination += (sender, args) => { WanderReached(); };

        base.Start();
    }

    #region Idle
    /// <summary>
    /// Stops the NPC from doing other actions.
    /// </summary>
    public override void Idle() {
        NPCMovementController.Stop();
    }
    #endregion

    #region Wander
    /// <summary>
    /// Indicates to the NPC to start wandering.
    /// </summary>
    public override void Wander() {
        // Set the first location to visit.
        NPCMovementController.SetLocation(wanderingPoints[Random.Range(0, wanderingPoints.Length)]);
    }

    /// <summary>
    /// How the NPC should react when a destination is reached while in
    /// the wandering state.
    /// </summary>
    private void WanderReached() {
        if (currentState != States.Wandering) {
            return;
        }

        if (stateCoroutine != null) {
            StopCoroutine(stateCoroutine);
        }

        stateCoroutine = StartCoroutine(WanderingWait(
           wanderingTime + Random.Range(-wanderingTimeDeviation, wanderingTimeDeviation)));
    }

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

    public override void Talk() {
    }

    public void Talk(GameObject gameObject) {
        UpdateState(States.Talking);
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
        UpdateToLastState();
        if (stateCoroutine != null) {
            StopCoroutine(stateCoroutine);
        }
        NPCMovementController.StopFace();
    }

    #endregion


    public override void Interact() {
    }

    public override void Attack() {
    }

    public override void Search() {
    }

}
