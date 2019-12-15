using System;
using UnityEngine;
/// <summary>
/// Controls the movement for the npc.
/// </summary>
public class NPCMovementController : MovementController {

    private Transform[] walkLocations;
    private int walkIndex = 0;

    private new void Update() {
        base.Update();

        // If the npc is walking, check to see if it should be heading to next destination.
        if (isWalking && !_navMeshAgent.pathPending && (_navMeshAgent.velocity.sqrMagnitude == 0f)) {
            ReachedDestination(this, new EventArgs());
            isWalking = false;
        }
    }

    public delegate void EventHandler(object sender, EventArgs args);
    public event EventHandler ReachedDestination = delegate { };

    public void SetLocation(Transform loc) {
        //_navMeshAgent.speed = speed;
        _navMeshAgent.isStopped = false;
        isWalking = true;
        _navMeshAgent.SetDestination(loc.position);
    }

    public void SetLocation(Transform[] locs) {
        if (locs.Length == 0) {
            return;
        }

        walkLocations = locs;
        walkIndex = 0;
        SetLocation(locs[walkIndex]);
    }
}
