using UnityEngine;

public class MouseStateController : MonoBehaviour {
    public static MouseStateController instance;

    public Texture2D cursorTexture;
    bool mouseState = false;


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
        mouseState = isActive;
        SetUIMouse(isActive);
    }

    public void SetUIMouse(bool isActive) {
        //if (mouseState || isActive) {
        //    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        //} else {
        //    Vector2 cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        //    Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        //}
    }
}
