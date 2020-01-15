using UnityEngine;

/// <summary>
/// All functions that are considered standard and needed inside of menus such as Quit.
/// </summary>
public class UIMenu_Standard : MonoBehaviour {

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
