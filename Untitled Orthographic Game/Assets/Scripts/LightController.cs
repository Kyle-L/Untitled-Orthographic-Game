using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour {

    private Light myLight;

    [Header("Fade in and out")]
    private float fadeIntensityOut = 0;
    private float fadeInIntensity;
    public float fadeSpeed = 1;

    [Header("Flicker")]
    public bool flicker = false;
    public float minFlickerIntensity = 0.25f;
    public float maxFlickerIntensity = 0.5f;
    public float flickerSpeed = 0.5f;
    private float tolerance = 0.01f;

    Coroutine lightCoroutine;

    void Start() {
        myLight = GetComponent<Light>();

        fadeInIntensity = myLight.intensity;

        if (flicker) {
            StartLight();
        }
    }

    IEnumerator LerpLight(float intensity, float speed) {
        while (Mathf.Abs(myLight.intensity - intensity) > tolerance) {
            myLight.intensity = Mathf.Lerp(myLight.intensity, intensity, speed * Time.deltaTime);
            yield return null;
        }
        if (flicker) {
            StartLight();
        }
    }

    private void StartLight() {
        StopAllCoroutines();
        lightCoroutine = StartCoroutine(LerpLight(Random.Range(minFlickerIntensity, maxFlickerIntensity), flickerSpeed));
    }

    public void FadeLightOut() {
        StopAllCoroutines();
        lightCoroutine = StartCoroutine(LerpLight(fadeIntensityOut, fadeSpeed));
    }

    public void FadeLightIn() {
        StopAllCoroutines();
        lightCoroutine = StartCoroutine(LerpLight(fadeInIntensity, fadeSpeed));
    }
}

