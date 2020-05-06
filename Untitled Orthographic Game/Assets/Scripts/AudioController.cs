using System.Collections;
using UnityEngine;

/// <summary>
/// A general controller for all of the audio sources
/// in a scene.
/// </summary>
public class AudioController : MonoBehaviour {
    public static AudioController instance;

    public AudioSource musicSource;
    public AudioSource effectSource;

    // Contains all of the audio sources in the game.
    private AudioSource[] sources;

    public float multiplier = 1f;
    public float fadeTolerance = 0.125f;
    public float fadeSpeed = 10f;

    bool transitioning = false;

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

        RefreshSources();
    }

    // Sets the volume of the audio listener.
    public void SetVolume(float volume) {
        AudioListener.volume = volume;
    }

    /// <summary>
    /// Fades the audio listener out completely according to fadeSpeed and
    /// fadeTolerance.
    /// </summary>
    public void FadeOut() {
        transitioning = true;
        StartCoroutine(FadeTo(0, fadeSpeed, fadeTolerance));
    }

    /// <summary>
    /// Fades the audio listener completely in according to fadeSpeed and
    /// fadeTolerance.
    /// </summary>
    public void FadeIn() {
        transitioning = true;
        StartCoroutine(FadeTo(multiplier, fadeSpeed, fadeTolerance));
    }

    /// <summary>
    /// The enumerator that handles audio fades.
    /// </summary>
    /// <param name="value">The value the audio listener is fading to.</param>
    /// <param name="speed">The speed the fade is handled at.</param>
    /// <param name="tolerance">The tolerance of the the fade stops.</param>
    /// <returns></returns>
    private IEnumerator FadeTo(float value, float speed, float tolerance) {
        while (Mathf.Abs(AudioListener.volume - value) > tolerance) {
            AudioListener.volume = Mathf.Lerp(AudioListener.volume, value, speed * Time.deltaTime);
            yield return null;
        }
        transitioning = false;
    }

    /// <summary>
    /// Returns whether the audio is fading in or out.
    /// </summary>
    /// <returns></returns>
    public bool GetTransition () {
        return transitioning;
    }

    /// <summary>
    /// Pauses all of the audio sources
    /// in the scene.
    /// </summary>
    public void Pause() {
        foreach (var a in sources) {
            a?.Pause();
        }
    }

    /// <summary>
    /// Unpauses all of the audio sources in
    /// the scene.
    /// </summary>
    public void Unpause() {
        foreach (var a in sources) {
            a?.UnPause();
        }
    }

    /// <summary>
    /// Refreshes all audio sources in the scene.
    /// </summary>
    public void RefreshSources() {
        sources = FindObjectsOfType<AudioSource>();
    }

    /// <summary>
    /// Plays a sound effect on the sound effect source.
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="volume"></param>
    public void PlayEffect(AudioClip effect, float volume = 1) {
        effectSource.PlayOneShot(effect, volume);
    }

}
