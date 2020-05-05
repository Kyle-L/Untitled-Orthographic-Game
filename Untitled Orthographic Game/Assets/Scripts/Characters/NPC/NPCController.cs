using UnityEngine;

/// <summary>
/// The overall controller for an npc.
/// </summary>
public class NPCController : Controller {

    // Components
    public NPCDialogueController NPCDialogueController { get; private set; }
    public NPCMovementController NPCMovementController { get; private set; }

    [Header("Optional- Pose")]
    public bool startPose = false;
    public string poseStr;
    public Transform pose;

    protected new void Start() {
        // Get componenets
        NPCDialogueController = GetComponent<NPCDialogueController>();
        NPCMovementController = GetComponent<NPCMovementController>();

        base.Start();
        
        if (startPose) {
            Pose(poseStr, pose);
        }
    }

}
