using UnityEngine;

/// <summary>
/// Put this componenet on the parent object of menu panels.
/// </summary>
public class UIMenu : MonoBehaviour {

    [SerializeField]
    [Tooltip("All of the ui objects that should be enabled/disabled when the ui is activated/deactivated.")]
    private GameObject[] uiObjects = null;
    [SerializeField]
    [Tooltip("Whether a background is enabled with this menu.")]
    private bool useMenuBackground = true;
    [SerializeField]
    [Tooltip("Whether the user can control the player while this menu is active.")]
    private bool enableControls = true;
    [SerializeField]
    [Tooltip("Whether time is paused when this menu is active.")]
    private bool doesPauseTime = true;
    [SerializeField]
    [Tooltip("Whether the user can go back to the last menu while this menu is active.")]
    private bool canGoBack = true;
    [SerializeField]
    [Tooltip("Whether another menu can be opened while this menu is active.")]
    private bool canOverride = false;
    [SerializeField]
    [Tooltip("Whether the menu has background music or not.")]
    private bool playBackgroundMusic = true;
    [SerializeField]
    [Tooltip("Whether the menu has effect sound or not.")]
    private bool playEffectSound = true;
    [SerializeField]
    private AudioClip overrideEffectClip;

    //[SerializeField]
    //[Tooltip("Whether the mouse is active while this menu is active.")]
    //private bool mouseState = true;

    /// <summary>
    /// Indicates whether this menu uses the default menu background.
    /// </summary>
    /// <returns></returns>
    public bool DoesUseMenuBackground() {
        return useMenuBackground;
    }

    /// <summary>
    ///Returns whether you are allowed to go back to prior menu from this menu.
    /// </summary>
    /// <returns></returns>
    public bool GetBackState() {
        return canGoBack;
    }

    /// <summary>
    /// Whether the user should be able to control while the menu is open.
    /// </summary>
    /// <returns></returns>
    public bool GetControlState() {
        return enableControls;
    }

    /// <summary>
    /// Whether the user should be able to control while the menu is open.
    /// </summary>
    /// <returns></returns>
    public bool GetPauseTime() {
        return doesPauseTime;
    }

    /// <summary>
    /// Returns whether another UI can overide this UI or not.
    /// </summary>
    /// <returns></returns>
    public bool GetOverrideState() {
        return canOverride;
    }

    /// <summary>
    /// Indicates whether this menu has background music or not.
    /// </summary>
    /// <returns></returns>
    public bool DoesHaveBackgroundMusic() {
        return playBackgroundMusic;
    }

    /// <summary>
    /// Indicates whether this menu has an effect sound or not.
    /// </summary>
    /// <returns></returns>
    public bool DoesPlayEffectSound() {
        return playEffectSound;
    }

    /// <summary>
    /// Returns the override effect sound if it exists, otherwise null.
    /// </summary>
    /// <returns></returns>
    public AudioClip GetEffectSound() {
        return overrideEffectClip;
    }

    public void Enable(bool doesEnable) {
        this.gameObject.SetActive(doesEnable);
        foreach (GameObject gameObject in uiObjects) {
            gameObject.SetActive(doesEnable);
        }
    }

}
