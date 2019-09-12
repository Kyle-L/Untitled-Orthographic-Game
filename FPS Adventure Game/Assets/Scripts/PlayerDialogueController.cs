using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Yarn.Unity;

public class PlayerDialogueController : MonoBehaviour {

    [SerializeField]
    private float interactionRadius = 2.0f;

    // Update is called once per frame
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
        var target = allParticipants.Find(delegate (NPCController p) {
            return string.IsNullOrEmpty(p.NPCDialogueController.TalkToNode) == false && // has a conversation node?
            (p.transform.position - this.transform.position)// is in range?
            .magnitude <= interactionRadius;
        });
        if (target != null) {
            // Kick off the dialogue at this node.
            DialogueRunner dr = FindObjectOfType<DialogueRunner>();

            // Quit the dialogue if it is already running.
            if (dr.isDialogueRunning) {
                return;
            }

            dr.StartDialogue(target.NPCDialogueController.TalkToNode);

            // Makes the npc face the player.
            // target.NPCMovementController.Face(PlayerController.instance.transform.position);

            // Makes the player face the npc.
            // PlayerController.instance.PlayerMovementController.Face(target.transform.position);
        }
    }
}
