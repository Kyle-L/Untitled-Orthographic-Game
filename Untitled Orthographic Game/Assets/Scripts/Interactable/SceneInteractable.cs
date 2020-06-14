using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInteractable : Interactable
{

    public int sceneNum;

    public override void Go(Controller controller) {
        base.Go(controller);
        GameManager.instance.LoadScene(sceneNum);
    }

    public override void Stop(Controller controller) {
        base.Stop(controller);
    }

}
