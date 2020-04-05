using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SubtitleTrigger : MonoBehaviour {
    [Multiline]
    public string text;

    private void OnTriggerEnter(Collider other) {
        UISubtitleController.instance.Add(text);
        gameObject.SetActive(false);
    }


}
