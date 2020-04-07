using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Yarn.Unity;

public class PlayerInteractionController : MonoBehaviour {

    public Text playerText;

    [SerializeField]
    private float interactionRadius = 1;
    private Coroutine interactionRange;

    private NPCController target;


    private void Start() {
        DialogueRunner.instance.DialogueFinished += (sender, args) => { StopDialogue(); };
    }

    void Update() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("Interactable")) {
                    Interactable objectHit = hit.transform.GetComponent<Interactable>();
                    if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {

                        PlayerControllerMain.instance.InteractWith(objectHit);
                    }
                } else if (hit.collider.CompareTag("Viewable")) {
                    Viewable objectHit = hit.transform.GetComponent<Viewable>();
                    if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                        PlayerControllerMain.instance.InteractWith(objectHit);
                    }
                } else if (hit.collider.CompareTag("Pickupable")) {
                    Pickupable objectHit = hit.transform.GetComponent<Pickupable>();
                    if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                        PlayerControllerMain.instance.InteractWith(objectHit);
                    }
                } else if (hit.collider.tag == "NPC") {
                    if (PlayerControllerMain.instance.Control) {
                        CheckForNearbyNPC(hit.collider.gameObject);
                    }
                } else if (hit.collider.tag == "Commentable") {
                    Commentable objectHit = hit.transform.GetComponent<Commentable>();
                    objectHit.Go();
                    if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                        DialogueRunner.instance.StartDialogue(objectHit.startNode, playerText);
                    }
                }


            }
        }
    }

    /// Find all DialogueParticipants
    /** Filter them to those that have a Yarn start node and are in range; 
     * then start a conversation with the first one
     */
    public void CheckForNearbyNPC(GameObject character) {
        target = character.GetComponentInParent<NPCController>();
        if (target != null) {
            // Quit the dialogue if it is already running.
            if (DialogueRunner.instance.isDialogueRunning) {
                return;
            }

            PlayerControllerMain.instance.InteractWith(target);

            if (interactionRange != null) {
                StopCoroutine(interactionRange);
            }
            interactionRange = StartCoroutine(WaitForInteractionRange(target));


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

    private IEnumerator WaitForInteractionRange(NPCController character) {
        while (Vector3.Distance(this.transform.position, character.transform.position) > interactionRadius) {
            if (!PlayerControllerMain.instance.MovementController.agentControlled) {
                yield break;
            }
            yield return null;
        }
        DialogueRunner.instance.StartDialogue(target.NPCDialogueController.talkToNode, target.NPCDialogueController.characterText);

        PlayerControllerMain.instance.Control = false;

        if (target.GetState() != Controller.States.Interacting) {
            target.InteractWith(PlayerControllerMain.instance);
        }
    }
}
