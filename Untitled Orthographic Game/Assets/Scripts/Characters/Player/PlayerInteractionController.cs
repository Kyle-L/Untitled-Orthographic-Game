using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Yarn.Unity;

public class PlayerInteractionController : MonoBehaviour {

    [SerializeField]
    private float interactionRadius = 2.0f;

    private NPCController target;

    private void Start() {
        DialogueRunner.instance.DialogueFinished += (sender, args) => { StopDialogue(); };
    }

    void Update() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
            if (Physics.Raycast(ray, out hit)) {
                Interactable objectHit = hit.transform.GetComponent<Interactable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {

                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            }
        }

        if (CrossPlatformInputManager.GetButtonDown("Interact")) {
            if (PlayerControllerMain.instance.Control) {
                CheckForNearbyNPC();
            }
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

            DialogueRunner.instance.StartDialogue(target.NPCDialogueController.talkToNode, target.NPCDialogueController.characterText);

            PlayerControllerMain.instance.Control = false;

            if (PlayerControllerMain.instance.GetState() != Controller.States.Interacting) {
                PlayerControllerMain.instance.InteractWith(target);
            }

            if (target.GetState() != Controller.States.Interacting) {
                target.InteractWith(PlayerControllerMain.instance);
            }

        }
    }

    private void StopDialogue() {
        if (target != null) {
            //target.currentState = Controller.States.Idle;
            target = null;
        }
        PlayerControllerMain.instance.Control = true;
        PlayerControllerMain.instance.MovementController.agentControlled = false;
        PlayerControllerMain.instance.SetState(Controller.States.UserControlled);
    }
}
