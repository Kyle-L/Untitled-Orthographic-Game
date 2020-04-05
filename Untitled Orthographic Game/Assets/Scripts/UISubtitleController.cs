using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class UISubtitleController : MonoBehaviour {
    [HideInInspector]
    public static UISubtitleController instance;

    public bool IsWriting { get; private set; }

    public const float WORDS_PER_MINUTE_MULTIPLIER_CHAR_TIME = 0.0025f;

    private int visible;

    //2D Subtitle
    public TextMeshProUGUI tm_text;
    public ScrollRect subtitleScrollRect;
    private Coroutine subtitle2DTextCoroutines;


    void Awake() {
        instance = this;
    }

    void Start() {
        tm_text.text = "";
    }

    [YarnCommand("add")]
    public void Add(string text) {
        text = text.Replace("\\n", "\n");
        text = "\n\t" + text;

        tm_text.text += text;

        IsWriting = true;
        subtitle2DTextCoroutines = StartCoroutine(TypeText());
    }

    IEnumerator TypeText(float charTime = WORDS_PER_MINUTE_MULTIPLIER_CHAR_TIME) {
        tm_text.maxVisibleCharacters = visible;
        while (tm_text.text.Length > visible) {
            visible++;
            tm_text.maxVisibleCharacters = visible;
            subtitleScrollRect.normalizedPosition = new Vector2(0, 0);

            float waitTime = WORDS_PER_MINUTE_MULTIPLIER_CHAR_TIME;
            yield return new WaitForSeconds(waitTime);
        }

        EndSubtitle();
    }

    public void EndSubtitle() {
        IsWriting = false;
        StopAllCoroutines();
    }

}