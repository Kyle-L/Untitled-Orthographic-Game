using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class MouseStateController : MonoBehaviour {
    public static MouseStateController instance;

    private bool mouseState = false;
    public bool mouseStateStart = false;

    Stack<bool> history;

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

        history = new Stack<bool>();

        Cursor.visible = false;
        mouseState = mouseStateStart;
    }

    public void GoBack () {
        if (history.Count == 0) {
            Cursor.visible = mouseStateStart;
            return;
        }
        SetMouseState(history.Pop(), false);
    }

    public void SetMouseState(bool isActive, bool addToHistory = true) {
        if (addToHistory) history.Push(mouseState);
        mouseState = isActive;

        Cursor.visible = mouseState;
    }

}
