using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The main controller of all the game's UI menus. 
/// Please Note this is a singleton class.
/// </summary>
public class UIMenuController : MonoBehaviour {
    public static UIMenuController instance;

    public Stack<GameObject> eventSystemHistory = new Stack<GameObject>();
    public EventSystem eventSystem;

    public bool UseEventSystem { get; set; } = false;

    [Header("General UI Settings")]
    // The default menu that is started on.
    public MainMenus defaultMenu = MainMenus.NoMenu;

    // Default Menus enum
    public enum MainMenus { NoMenu, MainMenu, PauseMenu, DialogueMenu, GameOver };

    // Default Menus
    public GameObject backgroundUI;
    public UIMenu mainMenuUI;
    public UIMenu pauseMenuUI;
    public UIMenu dialogueMenuUI;
    public UIMenu gameOverMenuUI;

    // The UI history. What menus the user has pressed on.
    private Stack<UIMenu> uiHistory = new Stack<UIMenu>();

    [Header("Audio Settings")]
    public AudioClip backgroundMusic;
    public AudioClip effectSound;

    public AudioSource uiMusicAudioSource;
    public AudioSource uiEffectAudioSource;

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

        if (uiMusicAudioSource == null) {
            uiMusicAudioSource = this.GetComponent<AudioSource>();
        }
        uiMusicAudioSource.playOnAwake = false;
        uiMusicAudioSource.clip = backgroundMusic;

        if (uiEffectAudioSource == null) {
            uiEffectAudioSource = this.GetComponent<AudioSource>();
        }
        uiEffectAudioSource.playOnAwake = false;
        uiEffectAudioSource.clip = effectSound;
    }

    public void Start() {
        // Set the default menu on start.
        if (GetCurrentMenu() == null) {
            SetMenu(defaultMenu);
        }
    }

    /// <summary>
    /// Returns to a previously active menu. If no there is no previous menu, then just shut the current menu.
    /// </summary>
    public void Back() {
        if (GetCurrentMenu() == null || !GetCurrentMenu().GetBackState()) {
            return;
        }

        // Pop the current menu from the stack and set it inactive.
        uiHistory.Pop().Enable(false);
        /* Set the current head of the stack to the active menu,
            * but don't add it to the UI history. */
        SetMenu(uiHistory.Count > 0 ? uiHistory.Peek() : null, false, eventSystemHistory.Count > 0 ? eventSystemHistory.Pop() : null);
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
    public void SetMenu(UIMenu aMenu, bool addToHistory, GameObject eventObject = null) {
        // If the history is greater than 0, disable the current head.
        if (uiHistory.Count > 0) {
            uiHistory.Peek().Enable(false);
        }

        /* If the menu is null or the menu is not null and the user can 
         * control while it is open. */
        if (aMenu == null || (aMenu != null && aMenu.GetControlState())) {
            // Then disable the mouse if it exists.
            MouseStateController.instance?.GoBack();
            CameraController.instance?.SetControl(true);
            GameManager.instance.ResumeTime();
            // Then unpause the player controller if it exists.
            if (PlayerControllerMain.instance != null) {
                PlayerControllerMain.instance.Pause = false;
            }
            // Otherwise
        } else {
            // Then enable the mouse if it exists.
            MouseStateController.instance?.SetMouseState(true, false);
            CameraController.instance?.SetControl(false);
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
            // Stop background music.
            uiMusicAudioSource?.Stop();

        // Otherwise
        } else {
            // Set the menu active.
            aMenu.Enable(true);
            // Use the background if the menu dictates it.
            backgroundUI.SetActive(aMenu.DoesUseMenuBackground());
            // Then stop time if menu dictates to pause time and the game manager exists.
            if (aMenu.GetPauseTime()) {
                GameManager.instance?.StopTime();
            } else {
                GameManager.instance?.ResumeTime();
            }
            // Play background music.
            if (aMenu.DoesHaveBackgroundMusic()) {
                if (!uiMusicAudioSource.isPlaying) {
                    uiMusicAudioSource.Play();
                }
            }
            // Play effect sound.
            if (aMenu.DoesPlayEffectSound()) {
                uiEffectAudioSource.PlayOneShot((aMenu.GetEffectSound() == null) ? effectSound : aMenu.GetEffectSound());
            }

            // Add the menu to history if the addToHistory param dictates it.
            if (addToHistory) {
                uiHistory.Push(aMenu);
                eventSystemHistory.Push(eventSystem.currentSelectedGameObject);
            }
        }

        if (UseEventSystem) {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(eventObject ?? aMenu?.GetDefaultButton());
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

}
