using UnityEngine;

public class Hideable : MonoBehaviour {
    public Vector3[] forwards;
    private MeshRenderer[] meshRenderers;

    void Start() {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();



    }

    void Update() {
        foreach (MeshRenderer meshRenderer in meshRenderers) {
            //meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Material[] materials = meshRenderer.materials;
            foreach (Material material in materials) {
                material.SetVector("_SectionPlane", new Vector4(0, 0, 0, 0));
            }
        }

        foreach (Vector3 v in forwards) {
            float dot = Vector3.Dot(Camera.main.transform.forward, v);
            foreach (MeshRenderer meshRenderer in meshRenderers) {
                if (dot >= 0) {
                    //meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    Material[] materials = meshRenderer.materials;
                    foreach (Material material in materials) {
                        material.SetVector("_SectionPlane", new Vector4(0, 1, 0, 0));
                    }
                }
            }

        }
    }
}
