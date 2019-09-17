﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Controls the movement for the npc.
/// </summary>
public class NPCMovementController : MonoBehaviour {

    [SerializeField]
    private float rotateSpeed = 5;

    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private Animator _animator;

    private Transform[] walkLocations;
    private int walkIndex = 0;

    //States
    private bool isWalking = false;

    private void Start() {

    }

    private void Update() {
        //_animator.SetFloat("Speed", _navMeshAgent.velocity.sqrMagnitude);

        // If the npc is walking, check to see if it should be heading to next destination.
        if (isWalking && !_navMeshAgent.pathPending) {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f) {
                    ReachedDestination(this, new EventArgs());
                    isWalking = false;
                }
            }
        }

    }

    public delegate void EventHandler(object sender, EventArgs args);
    public event EventHandler ReachedDestination = delegate { };

    public void Stop() {
        _navMeshAgent.isStopped = true;
    }

    public void SetLocation(Transform loc) {
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

    /// <summary>
    /// Makes the npc face a target position.
    /// </summary>
    /// <param name="target"></param>
    public void Face(Vector3 target) {
        // Stops the npc from being able to move.
        _navMeshAgent.isStopped = true;

        // Calculates the direction by finding the normalized difference.
        Vector3 direction = (target - transform.position).normalized;

        // Converts the direction to a quaternion.
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        /* Sets the x and y rotations to 0 so that the npc only turns on
           the y axis.*/
        lookRotation.x = lookRotation.z = 0;

        // If a coroutine is running, stop it.
        if (lookCoroutine != null) {
            StopCoroutine(lookCoroutine);
        }

        // Start the look look at coroutine.
        lookCoroutine = StartCoroutine(LookAtCoroutine(lookRotation));
    }

    Coroutine lookCoroutine;
    /// <summary>
    /// The coroutine the rotates the npc based on the angle.
    /// </summary>
    /// <param name="angle">The angle the npc is rotating towards</param>
    /// <returns></returns>
    private IEnumerator LookAtCoroutine(Quaternion angle) {
        while (Quaternion.Angle(transform.rotation, angle) > 1) {
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
