using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(Animator))]
public class YarnAnimator : MonoBehaviour {

    public string[] gestureAnimationNames;

    private Animator _animator;

    private int layerIndex;

    private void Start() {
        _animator = GetComponent<Animator>();

        layerIndex = _animator.GetLayerIndex("Gestures");
    }

    [YarnCommand("gesture")]
    public void Gesture(string gestureName) {
        foreach (string str in gestureAnimationNames) {
            if (gestureName.Equals(str)) {
                _animator.Play(str, layerIndex);
                return;
            }
        }
        print("There is no state \"" + gestureName + "\" on layer \"" +
              layerIndex + "\" of \"" + this.name + "\"");
    }
}
