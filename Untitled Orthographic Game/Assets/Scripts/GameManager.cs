using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public float minimumLoadTime = 1.5f;

    public ScriptedEvent startEvent;

    private int prevScene = -1;

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

        //ResumeTime();
    }

    private void Start() {
        startEvent?.Go();
        AudioController.instance.FadeIn();
    }

    public void StopTime() {
        Time.timeScale = 0;
        //PlayerController.instance?.Pause(true);
        //CameraController.instance?.SetControl(false);
        AudioController.instance?.RefreshSources();
        AudioController.instance?.Pause(); ;
    }

    public void ResumeTime() {
        Time.timeScale = 1;
        //PlayerController.instance?.Pause(false);
        //CameraController.instance?.SetControl(true);
        AudioController.instance?.Unpause();
    }

    Coroutine sceneLoadCoroutine;
    public void LoadScene(int num) {
        if (sceneLoadCoroutine != null) {
            return;
        }

        prevScene = SceneManager.GetActiveScene().buildIndex;
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(num);
        sceneLoadCoroutine = StartCoroutine(LoadSceneCo(num));
    }

    public void LoadNextScene() {
        LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadSceneForce(int num) {
        if (sceneLoadCoroutine != null) {
            return;
        }

        // Ensures the user can't pause the game once a load has started.
        UIMenuController.instance.SetPauseState(false);

        prevScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(num);
    }

    private IEnumerator LoadSceneCo(int num) {
        UIFadeTransitionController.instance.FadeIn();
        AudioController.instance.FadeOut();

        while (UIFadeTransitionController.instance.GetTransition() || AudioController.instance.GetTransition()) {
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(num);
        asyncLoad.allowSceneActivation = false;

        float loadTime = minimumLoadTime;
        while (asyncLoad.progress < 0.9f || loadTime > 0) {
            loadTime -= Time.deltaTime;
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }

    public void ReloadScene() {
        prevScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public int GetPreviousScene() {
        return prevScene;
    }

    public int GetCurrentScene() {
        return SceneManager.GetActiveScene().buildIndex;
    }

}
