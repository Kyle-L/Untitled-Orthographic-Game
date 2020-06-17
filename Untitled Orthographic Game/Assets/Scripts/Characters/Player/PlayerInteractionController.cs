using Boo.Lang;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class PlayerInteractionController : MonoBehaviour {

    public Text playerText;

    [SerializeField]
    private float interactionRadius = 1;
    private Coroutine interactionRange;

    private NPCController target;


    private void Start() {
        DialogueRunner.instance.DialogueFinished += (sender, args) => { StopDialogue(); };

        list = new List<InteractBase>();
    }

    private InteractBase last;
    private InteractBase cur;

    void Update() {

        if (Input.GetButtonDown("Fire1") && cur != null) {
            if (cur.CompareTag("Interactable")) {
                Interactable objectHit = cur.transform.GetComponent<Interactable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {

                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            } else if (cur.CompareTag("Viewable")) {
                Viewable objectHit = cur.transform.GetComponent<Viewable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            } else if (cur.CompareTag("Pickupable")) {
                Pickupable objectHit = cur.transform.GetComponent<Pickupable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            } else if (cur.tag == "NPC") {
                if (PlayerControllerMain.instance.Control) {
                    CheckForNearbyNPC(cur.gameObject);
                }
            } else if (cur.tag == "Commentable") {
                Commentable objectHit = cur.transform.GetComponent<Commentable>();
                objectHit.Go();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                    DialogueRunner.instance.StartDialogue(objectHit.startNode, playerText);
                }
            }
            cur.DisableUI();
        }

        InteractBase bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (InteractBase potentialTarget in list) {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr) {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        cur = bestTarget;
        if (last != cur) {
            last?.DefocusUI();
            cur.FocusUI();
            last = cur;
        }
    }

    private List<InteractBase> list;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Pickupable") || other.CompareTag("Interactable") || other.CompareTag("Viewable") ||
            other.CompareTag("Commentable") || other.CompareTag("NPC")) {
            list.Add(CharacterSerializer.instance.InteractDictionary[other.gameObject]);
            CharacterSerializer.instance.InteractDictionary[other.gameObject].EnableUI();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Pickupable") || other.CompareTag("Interactable") || other.CompareTag("Viewable") ||
            other.CompareTag("Commentable") || other.CompareTag("NPC")) {
            list.Remove(CharacterSerializer.instance.InteractDictionary[other.gameObject]);
            CharacterSerializer.instance.InteractDictionary[other.gameObject].DisableUI();
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
        target.NPCDialogueController.EnableUI();

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
            //if (!PlayerControllerMain.instance.MovementController.agentControlled) {
            //    print("break");
            //    yield break;
            //}
            print("waiting");
            yield return null;
        }
        DialogueRunner.instance.StartDialogue(target.NPCDialogueController.talkToNode, target.NPCDialogueController.characterText);

        PlayerControllerMain.instance.Control = false;

        if (target.GetState() != Controller.States.Interacting) {
            target.InteractWith(PlayerControllerMain.instance);
        }
    }
}
