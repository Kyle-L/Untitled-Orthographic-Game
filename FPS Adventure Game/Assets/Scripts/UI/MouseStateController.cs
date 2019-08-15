using UnityEngine;

public class MouseStateController : MonoBehaviour {
    public static MouseStateController instance;

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

    public void SetMouseState(bool isActive) {
        Cursor.visible = isActive;
        Cursor.lockState = (isActive) ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
