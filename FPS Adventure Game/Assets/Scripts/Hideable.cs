using UnityEngine;

public class Hideable : MonoBehaviour {
    public Vector3 forwards;
    [Range(0, 10)]
    public float crossSectionMaxHeight = 4;
    [Range(0, 10)]
    public float crossSectionMinHeight = 0.1f;
    private MeshRenderer[] meshRenderers;
    public bool alwaysHide = false;
    private float dot, lastDot;

    void Start() {
        // Gets all mesh renders in the object.
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        GameObject g = Instantiate(this.gameObject, this.transform.parent);
        Destroy(g.GetComponent<Hideable>());
        MeshRenderer[] meshRenderersNew = g.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenderersNew) {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }

        foreach (MeshRenderer meshRenderer in meshRenderers) {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            Material[] materials = meshRenderer.materials;
            foreach (Material material in materials) {
                material.SetVector("_SectionPoint", new Vector4(0, crossSectionMinHeight, 0, 0));
            }
        }
    }

    void Update() {
        if (!alwaysHide) {
            dot = Vector3.Dot(Camera.main.transform.forward, forwards);

            if (lastDot * dot <= 0) {
                foreach (MeshRenderer meshRenderer in meshRenderers) {
                    if (dot >= 0) {
                        Material[] materials = meshRenderer.materials;
                        foreach (Material material in materials) {
                            material.SetVector("_SectionPoint", new Vector4(0, crossSectionMaxHeight, 0, 0));
                        }
                    } else {
                        Material[] materials = meshRenderer.materials;
                        foreach (Material material in materials) {
                            material.SetVector("_SectionPoint", new Vector4(0, crossSectionMinHeight, 0, 0));
                        }
                    }
                }
            }
            lastDot = dot;
        }
    }
}
