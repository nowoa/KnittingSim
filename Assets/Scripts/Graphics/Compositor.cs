using System;
using UnityEngine;


public class Compositor : MonoBehaviour
{
    [SerializeField] private CameraClone cameraClone;
    
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Color outlineColor = Color.black;

    private Material _outlineMaterial;
    
    private void OnEnable()
    {
        _outlineMaterial = new Material(outlineMaterial);
        
        UpdateRenderTexture(cameraClone.RenderTexture);
        cameraClone.onUpdateTexture.AddListener(UpdateRenderTexture);
        
    }

    private void OnDisable()
    {
        cameraClone.onUpdateTexture.RemoveListener(UpdateRenderTexture);
    }

    private void UpdateRenderTexture(RenderTexture rt)
    {
        _outlineMaterial.SetTexture("_MaskTex", rt);
    }

    private void Update()
    {
        _outlineMaterial.SetColor("_OutlineColor", outlineColor);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _outlineMaterial);
    }
}
