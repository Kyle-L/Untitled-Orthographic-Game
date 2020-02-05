using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Yarn.Unity;

public class PlayerDialogueController : MonoBehaviour {

    [SerializeField]
    private float interactionRadius = 2.0f;

    public GameObject head;

    private NPCController target;

    private void Start() {
        DialogueRunner.instance.DialogueFinished += (sender, args) => { StopDialogue(); };
    }

    void Update() {
        if (CrossPlatformInputManager.GetButtonDown("Interact")) {
            CheckForNearbyNPC();
        }
    }

    /// Find all DialogueParticipants
    /** Filter them to those that have a Yarn start node and are in range; 
     * then start a conversation with the first one
     */
    public void CheckForNearbyNPC() {
        var allParticipants = new List<NPCController>(FindObjectsOfType<NPCController>());
        target = allParticipants.Find(delegate (NPCController p) {
            return string.IsNullOrEmpty(p.NPCDialogueController.talkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null) {
            // Quit the dialogue if it is already running.
            if (DialogueRunner.instance.isDialogueRunning) {
                return;
            }

            DialogueRunner.instance.StartDialogue(target.NPCDialogueController.talkToNode);


            PlayerControllerMain.instance.MovementController.agentControlled = true;
            PlayerControllerMain.instance.ModifyBlackBoard(Controller.BlackBoardVars.InteractingObject, target);
            PlayerControllerMain.instance.currentState = Controller.States.Interacting;

            target.ModifyBlackBoard(Controller.BlackBoardVars.InteractingObject, PlayerControllerMain.instance);
            target.currentState = Controller.States.Interacting;
        }
    }

    private void StopDialogue() {
        if (target != null) {
            target.currentState = Controller.States.Idle;
            target = null;
        }
        PlayerControllerMain.instance.currentState = Controller.States.UserControlled;
    }
}
