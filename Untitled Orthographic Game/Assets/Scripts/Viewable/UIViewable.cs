using UnityEngine;
using Yarn;

public class UIViewable : Viewable {

    public string viewableName;
    public TextAsset viewableText;

    public override void Go() {
        base.Go();

        UIMenu_Viewable.instance.View(this);
    }

}
