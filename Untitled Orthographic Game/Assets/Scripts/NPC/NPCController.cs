﻿using NPBehave;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The overall controller for an npc.
/// </summary>
public class NPCController : Controller {

    // Components
    public NPCDialogueController NPCDialogueController { get; private set; }
    public NPCMovementController NPCMovementController { get; private set; }

    protected new void Start() {
        // Get componenets
        NPCDialogueController = GetComponent<NPCDialogueController>();
        NPCMovementController = GetComponent<NPCMovementController>();

        base.Start();
    }

}
