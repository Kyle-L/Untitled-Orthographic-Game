using UnityEngine;

public class UIViewable : Viewable {

    public string viewableName;
    public TextAsset viewableText;

    public override void Go() {
        UIMenu_Viewable.instance.View(this);
    }

    public override void Stop() {

    }
}
