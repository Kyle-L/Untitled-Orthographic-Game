using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// The main controller of all the game's UI menus. 
/// Please Note this is a singleton class.
/// </summary>
public class UIMenuController : MonoBehaviour {
    public static UIMenuController instance;

    // Whether the user can pause the game.
    private bool canPause = true;

    // Default Menus enum
    public enum MainMenus { NoMenu, MainMenu, PauseMenu, DialogueMenu, GameOver };

    // The default menu that is started on.
    public MainMenus defaultMenu = MainMenus.NoMenu;

    // Default Menus
    public GameObject backgroundUI;
    public UIMenu mainMenuUI;
    public UIMenu pauseMenuUI;
    public UIMenu dialogueMenuUI;
    public UIMenu gameOverMenuUI;

    // The UI history. What menus the user has pressed on.
    private Stack<UIMenu> uiHistory = new Stack<UIMenu>();

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

    public void Start() {
        // Set the default menu on start.
        SetMenu(defaultMenu);
    }

    private void Update() {
        // If the user hits the cancel key and the game is able to be paused.
        if (CrossPlatformInputManager.GetButtonDown("Cancel") && canPause) {
            // If the menu is not active.
            if (!GetMenuState() || GetOverrideState()) {
                // Enable the pause menu.
                SetMenu(MainMenus.PauseMenu);
                // Otherwise, can the current menu go back.
            } else if (GetCurrentMenu().GetBackState()) {
                // Then go back if it can.
                Back();
            }
        }
    }

    /// <summary>
    /// Returns to a previously active menu. If no there is no previous menu, then just shut the current menu.
    /// </summary>
    public void Back() {
        // If the current menu is the last.
        if (uiHistory.Count <= 1) {
            // Then disable the menu.
            SetMenu(MainMenus.NoMenu);
            // If the menu is not the last.
        } else {
            // Pop the current menu from the stack and set it inactive.
            uiHistory.Pop().gameObject.SetActive(false);
            /* Set the current head of the stack to the active menu,
             * but don't add it to the UI history. */
            SetMenu(uiHistory.Peek(), false);
        }
    }

    /// <summary>
    /// Opens the pause menu.
    /// </summary>
    public void PauseMenu() {
        SetMenu(MainMenus.PauseMenu);
    }

    /// <summary>
    /// Closes the current menu.
    /// </summary>
    public void CloseMenu() {
        SetMenu(MainMenus.NoMenu);
    }

    /// <summary>
    /// Sets a menu to active.
    /// </summary>
    /// <param name="aMenu">The menu you would like to set active. Please note see the overloaded method to send a Menu object rather than enumerated type.</param>
    public void SetMenu(MainMenus aMenu) {
        switch (aMenu) {
            case MainMenus.MainMenu:
                SetMenu(mainMenuUI);
                break;
            case MainMenus.PauseMenu:
                SetMenu(pauseMenuUI);
                break;
            case MainMenus.DialogueMenu:
                SetMenu(dialogueMenuUI);
                break;
            case MainMenus.GameOver:
                SetMenu(gameOverMenuUI);
                break;
            default:
                SetMenu(null);
                break;
        }
    }

    /// <summary>
    /// Sets a menu to active. 
    /// </summary>
    /// <param name="aMenu">The menu you would like to set active.</param>
    public void SetMenu(UIMenu aMenu) {
        SetMenu(aMenu, true);
    }

    /// <summary>
    /// Sets a menu to active. 
    /// </summary>
    /// <param name="aMenu">The menu you would like to set active.</param>
    public void SetMenu(UIMenu aMenu, bool addToHistory) {
        // If the history is greater than 0, disable the current head.
        if (uiHistory.Count > 0) {
            uiHistory.Peek().gameObject.SetActive(false);
        }

        /* If the menu is null or the menu is not null and the user can 
         * control while it is open. */
        if (aMenu == null || (aMenu != null && aMenu.GetControlState())) {
            // Then disable the mouse if it exists.
            MouseStateController.instance?.SetUIMouse(false);
            GameManager.instance.ResumeTime();
            // Then unpause the player controller if it exists.
            if (PlayerControllerMain.instance != null) {
                PlayerControllerMain.instance.Pause = false;
            }
            // Otherwise
        } else {
            // Then enable the mouse if it exists.
            MouseStateController.instance?.SetUIMouse(true);
            GameManager.instance.StopTime();
            // Then pause the player controller if it exists.
            if (PlayerControllerMain.instance != null) {
                PlayerControllerMain.instance.Pause = true;
            }
        }

        // If the the current menu is null.
        if (aMenu == null) {
            // Clear the history.
            uiHistory.Clear();
            // Disable the background.
            backgroundUI.SetActive(false);
            // Resumes time.
            GameManager.instance?.ResumeTime();
            // Otherwise
        } else {
            // Set the menu active.
            aMenu.gameObject.SetActive(true);
            // Use the background if the menu dictates it.
            backgroundUI.SetActive(aMenu.DoesUseMenuBackground());
            // Then stop time if menu dictates to pause time and the game manager exists.
            if (aMenu.GetPauseTime()) {
                GameManager.instance?.StopTime();
            } else {
                GameManager.instance?.ResumeTime();
            }
            // Add the menu to history if the addToHistory param dictates it.
            if (addToHistory) {
                uiHistory.Push(aMenu);
            }
        }
    }

    /// <summary>
    /// Returns the current active menu.
    /// </summary>
    /// <returns>The active menu, null if no menu is active.</returns>
    public UIMenu GetCurrentMenu() {
        return (uiHistory.Count > 0) ? uiHistory.Peek() : null;
    }

    /// <summary>
    /// Returns whether a menu is currently active.
    /// </summary>
    /// <returns>True if a menu is active, false otherwise.</returns>
    public bool GetMenuState() {
        return uiHistory.Count > 0;
    }

    public bool GetOverrideState() {
        UIMenu cur = GetCurrentMenu();
        if (cur != null) {
            return cur.GetOverrideState();
        }
        return false;
    }

    /// <summary>
    /// Whether the user can pause the game.
    /// </summary>
    /// <param name="canPause"></param>
    public void SetPauseState(bool canPause) {
        this.canPause = canPause;
    }

}
