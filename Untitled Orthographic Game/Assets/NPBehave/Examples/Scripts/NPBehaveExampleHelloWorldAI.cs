using NPBehave;
using UnityEngine;

public class NPBehaveExampleHelloWorldAI : MonoBehaviour {
    private Root behaviorTree;

    void Start() {
        behaviorTree = new Root(
            new Action(() => Debug.Log("Hello World!"))
        );
        behaviorTree.Start();
    }
}
