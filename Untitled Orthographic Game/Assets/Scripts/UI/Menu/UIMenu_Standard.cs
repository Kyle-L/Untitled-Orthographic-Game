using UnityEngine;

/// <summary>
/// All functions that are considered standard and needed inside of menus such as Quit.
/// </summary>
public class UIMenu_Standard : MonoBehaviour {
    public static UIMenu_Standard instance;

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

    /// <summary>
    /// Shuts down the application.
    /// </summary>
	public void Quit() {
        Application.Quit();
    }

    /// <summary>
    /// Sets the current scene.
    /// </summary>
    /// <param name="num"></param>
    public void SetScene(int num) {
        GameManager.instance.LoadScene(num);
    }

    /// <summary>
    /// Sets a current scene instantly. No transition
    /// or load screen.
    /// </summary>
    /// <param name="num"></param>
    public void SetSceneForce(int num) {
        GameManager.instance.LoadSceneForce(num);
    }

    /// <summary>
    /// Resets the current scene.
    /// </summary>
    public void ResetScene() {
        GameManager.instance.ReloadScene();
    }

}
