using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Yarn.Unity;

public class PlayerDialogueController : MonoBehaviour {

    [SerializeField]
    private float interactionRadius = 2.0f;

    private DialogueRunner dr;

    private NPCController target;

    private void Start() {
        dr = FindObjectOfType<DialogueRunner>();

        dr.DialogueFinished += (sender, args) => { StopDialogue(); };
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
            return string.IsNullOrEmpty(p.NPCDialogueController.TalkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null) {
            // Quit the dialogue if it is already running.
            if (dr.isDialogueRunning) {
                return;
            }

            dr.StartDialogue(target.NPCDialogueController.TalkToNode);

            target.Talk(this.gameObject);
        }
    }

    private void StopDialogue () {
        if (target != null) {
            target.StopTalk();
            target = null;
        }
    }
}
