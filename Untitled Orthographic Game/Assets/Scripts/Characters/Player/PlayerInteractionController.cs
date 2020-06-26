using System.Collections.Generic;
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

        if (Input.GetButtonDown("Fire1") && cur != null && !UIMenuController.instance.GetMenuState() && !DialogueRunner.instance.isDialogueRunning) {
            if (cur.CompareTag("Interactable")) {
                cur.DisableUI();
                Interactable objectHit = cur.transform.GetComponent<Interactable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {

                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            } else if (cur.CompareTag("Viewable")) {
                cur.DisableUI();
                Viewable objectHit = cur.transform.GetComponent<Viewable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            } else if (cur.CompareTag("Pickupable")) {
                cur.DisableUI();
                Pickupable objectHit = cur.transform.GetComponent<Pickupable>();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                    PlayerControllerMain.instance.InteractWith(objectHit);
                }
            } else if (cur.tag == "NPC") {
                cur.DisableUI();
                if (PlayerControllerMain.instance.Control) {
                    CheckForNearbyNPC(cur.gameObject);
                }
            } else if (cur.tag == "Commentable") {
                cur.DisableUI();
                Commentable objectHit = cur.transform.GetComponent<Commentable>();
                objectHit.Go();
                if (objectHit != null && !DialogueRunner.instance.isDialogueRunning) {
                    DialogueRunner.instance.StartDialogue(objectHit.startNode, playerText);
                }
            }
        }

        InteractBase bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = PlayerControllerMain.instance.MovementController.HeadIKController.head.position;
        //foreach (InteractBase potentialTarget in list) {
        //    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
        //    float dSqrToTarget = directionToTarget.sqrMagnitude;
        //    if (dSqrToTarget < closestDistanceSqr) {
        //        closestDistanceSqr = dSqrToTarget;
        //        bestTarget = potentialTarget;
        //    }
        //}
        bestTarget = PlayerControllerMain.instance.MovementController.HeadIKController.GetClosestInFrontTransform(list);

        cur = bestTarget;
        if (last != cur) {
            last?.DefocusUI();
            cur?.FocusUI();
            last = cur;
        }
    }

    private List<InteractBase> list;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Pickupable") || other.CompareTag("Interactable") || other.CompareTag("Viewable") ||
            other.CompareTag("Commentable") || other.CompareTag("NPC")) {
            if (CharacterSerializer.instance.InteractDictionary.TryGetValue(other.gameObject, out InteractBase outVal)) {
                list.Add(outVal);
                outVal.UnhideUI();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Pickupable") || other.CompareTag("Interactable") || other.CompareTag("Viewable") ||
            other.CompareTag("Commentable") || other.CompareTag("NPC")) {
            list.Remove(CharacterSerializer.instance.InteractDictionary[other.gameObject]);
            CharacterSerializer.instance.InteractDictionary[other.gameObject].HideUI();
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
            cur.EnableUI();
        }
        PlayerControllerMain.instance.Control = true;
        PlayerControllerMain.instance.MovementController.agentControlled = false;
        PlayerControllerMain.instance.SetState(Controller.States.UserControlled);
    }

    private void SetUI(bool state) {
        foreach (InteractBase interactBase in list) {
            if (state) {
                interactBase.UnhideUI();
            } else {
                interactBase.HideUI();
            }
        }
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
