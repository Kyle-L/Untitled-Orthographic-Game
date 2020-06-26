using System;
using UnityEngine;

public class CharacterTrack : MonoBehaviour {

    public Shader _drawShader;

    private RenderTexture _splatmap;
    private Material _snowMaterial, _drawMaterial;

    [Range(0, 500)]
    public float _brushSize;
    [Range(0, 1)]
    public float _brushStrength;

    public GameObject _terrain;
    public Transform[] feet;

    private RaycastHit _hit;
    int _layerMask;

    private void Start() {
        // If the terrain is null or there is no feet, then no tracks can be rendered.
        if (_terrain == null || feet.Length == 0) {
            print("No terrain or feet are set. " + this.name + " is unable to render tracks.");
            return;
        }

        _layerMask = LayerMask.GetMask("Ground");

        _drawMaterial = new Material(_drawShader);
        _drawMaterial.SetVector("_Color", Color.red);

        _snowMaterial = _terrain.GetComponent<MeshRenderer>().material;
        if (_snowMaterial.GetTexture("_Splat") == null) {
            _splatmap = new RenderTexture(2056, 2056, 0, RenderTextureFormat.ARGBFloat);
        } else {
            _splatmap = (RenderTexture)_snowMaterial.GetTexture("_Splat");
        }
        _snowMaterial.SetTexture("_Splat", _splatmap);
    }

    private void Update() {
        // If the terrain is null or there is no feet, then no tracks can be rendered.
        if (_terrain == null || feet.Length == 0) {
            return;
        }

        foreach (Transform tran in feet) {
            if (Physics.Raycast(tran.position, Vector3.down, out _hit, 1f, _layerMask)) {
                _drawMaterial.SetVector("_Coordinate", new Vector4(_hit.textureCoord.x, _hit.textureCoord.y, 0, 0));
                _drawMaterial.SetFloat("_Strength", _brushStrength);
                _drawMaterial.SetFloat("_Size", _brushSize);

                RenderTexture temp = RenderTexture.GetTemporary(_splatmap.width, _splatmap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_splatmap, temp);
                Graphics.Blit(temp, _splatmap, _drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }
}
