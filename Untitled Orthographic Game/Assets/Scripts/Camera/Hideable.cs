using UnityEngine;

public class Hideable : MonoBehaviour {

    [Header("Forward Axis Settings")]
    public Vector3 forwardAxis;

    [Header("Hide Settings")]
    public bool alwaysHide = false;
    public GameObject[] hideObjects;

    [Header("Cross Section Settings")]
    [Range(0, 10)]
    public float crossSectionMaxHeight = 4;
    [Range(0, 10)]
    public float crossSectionMinHeight = 0.1f;

    private MeshRenderer[] meshRenderers;
    private float dot, lastDot;

    private void Start() {
        // Gets all mesh renders in the object.
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        // Creates a copy of this hideable.
        GameObject g = Instantiate(this.gameObject, this.transform.parent);

        // Destroy the hideable componenet on the copy to avoid infinite recursion.
        Destroy(g.GetComponent<Hideable>());

        // Find all mesh renders in the copy.
        MeshRenderer[] meshRenderersNew = g.GetComponentsInChildren<MeshRenderer>();

        /* Turns the casting mode to shadow only on all mesh renders of the copy.
         * This makes sure when the object is hidden, shadows do not abruptly change.*/
        foreach (MeshRenderer meshRenderer in meshRenderersNew) {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        /* Turns the mesh renders of the gameobject to off so they do not confict with the
         * shadows from the copy. */
        foreach (MeshRenderer meshRenderer in meshRenderers) {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // Hides all hideable by default.
            Hide(true);
        }
    }

    private void Update() {
        // If always hide is not active. Check the dot product against the main camera.
        if (!alwaysHide) {
            dot = Vector3.Dot(Camera.main.transform.forward, forwardAxis);

            /* If the dot product has indicat a change in direction.
             * The change is detected by multiplying the current dot product
             * by the last one. If the result is less than or equal to 0, then
             * the direction has changed.*/
            if (lastDot * dot <= 0) {
                /* If the dot product is greater than or equal to 0, then 
                 * it should be unhidden since the forward is same
                 * direction of the camera.*/
                if (dot >= 0) {
                    Hide(false);

                    /* If the dot product is less than 0, then 
                     * it should be hidden since the forward is the opposite
                     * direction of the camera.*/
                } else {
                    Hide(true);
                }
            }

            // Update the last dot to the current.
            lastDot = dot;
        }
    }

    /// <summary>
    /// Hides the current hideable.
    /// </summary>
    /// <param name="doesHide"></param>
    private void Hide(bool doesHide) {
        // Interates through all mesh renders.
        foreach (MeshRenderer meshRenderer in meshRenderers) {
            // Iterates through each material.
            Material[] materials = meshRenderer.materials;
            foreach (Material material in materials) {
                // If does hide is true, set the cross section to the hide height.
                if (doesHide) {
                    material.SetVector("_SectionPoint", new Vector4(0, crossSectionMinHeight, 0, 0));
                    // If does hide is false, set the cross section to the max height.
                } else {
                    material.SetVector("_SectionPoint", new Vector4(0, crossSectionMaxHeight, 0, 0));
                }
            }
            // Iterates through each game in hide objects.
            foreach (GameObject gameObject in hideObjects) {
                /* If does hide is true, hide the object by setting it inactive. 
                 * If does hide is false, hide the object by setting it active. */
                gameObject.SetActive(!doesHide);
            }
        }
    }
}
