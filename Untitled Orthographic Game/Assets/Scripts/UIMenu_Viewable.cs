using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All functions that are considered for the viewable interactions.
/// </summary>
public class UIMenu_Viewable : MonoBehaviour {
    public static UIMenu_Viewable instance;

    public UIMenu menu;
    public Text viewableUITextTitle;
    public Text viewableUITextBody;

    public GameObject backButton;
    public GameObject leftButton;
    public GameObject rightButton;

    private UIViewable currentView;

    private void Awake() {
        #region Enforces Singleton Pattern.
        //Check if instance already exists
        if (instance == null) {
            //if not, set instance to this	
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a MyNetworkManager.
            Destroy(gameObject);
        }
        #endregion
    }

    public void View(UIViewable text) {
        currentView = text;
        viewableUITextTitle.text = text.viewableName;
        viewableUITextBody.text = text.viewableText.text;
        UIMenuController.instance.SetMenu(menu);

        leftButton.SetActive(text.leftView != null);
        rightButton.SetActive(text.rightView != null);
    }

    public void ViewLeft() {
        View((UIViewable)currentView.leftView);
    }

    public void ViewRight() {
        View((UIViewable)currentView.rightView);
    }

}
