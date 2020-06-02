using UnityEngine;
using UnityEngine.UI;

public class MouseStateController : MonoBehaviour {
    public static MouseStateController instance;

    public GameObject cursor;
    public Text text;
    public CanvasGroup cg;
    bool mouseState = false;

    private GameObject lastHit;

    public Animator animator;

    private void Awake() {
        if (instance == null) {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a UICrossHairController.
            Destroy(gameObject);
            return;
        }
    }

    public void Update() {
        cursor.transform.position = Input.mousePosition;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {

            Debug.DrawLine(Camera.main.transform.position, hit.point);

            if (hit.transform.gameObject != lastHit) {
                lastHit = hit.transform.gameObject;

                if (lastHit.tag == "Interactable") {
                    animator.SetBool("Interacting", true);
                    Interactable i = lastHit.GetComponent<Interactable>();
                    text.text = i.interactionUIActionString + " " + i.interactionUIObjectString;
                } else if (lastHit.tag == "Viewable") {
                    animator.SetBool("Interacting", true);
                    Viewable i = lastHit.GetComponent<Viewable>();
                    text.text = i.viewUIActionString + " " + i.viewUIObjectString;
                } else if (lastHit.tag == "Pickupable") {
                    animator.SetBool("Interacting", true);
                    Pickupable i = lastHit.GetComponent<Pickupable>();
                    text.text = i.pickUpUIActionString + " " + i.pickUpUIObjectString;
                } else if (lastHit.tag == "NPC") {
                    animator.SetBool("Interacting", true);
                    Controller i = lastHit.GetComponentInParent<Controller>();
                    text.text = "Talk to " + i.name;
                } else if (lastHit.tag == "Commentable") {
                    animator.SetBool("Interacting", true);
                    Commentable i = lastHit.GetComponentInParent<Commentable>();
                    text.text = "Comment on " + i.commentableUIObjectString;
                } else {
                    animator.SetBool("Interacting", false);
                }
            }
        } else {
            animator.SetBool("Interacting", false);
        }
    }

    public void SetMouseState(bool isActive) {
        mouseState = isActive;
        SetUIMouse(isActive);
    }

    public void SetUIMouse(bool isActive) {
        if (mouseState || isActive) {
            Cursor.visible = true;
            cg.alpha = 0;
        } else {
            Cursor.visible = false;
            cg.alpha = 1;
        }
    }
}
