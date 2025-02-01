using System;
using UnityEngine;


public class Compositor : MonoBehaviour
{
    [SerializeField] private CameraClone cameraClone;
    
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Color outlineColor = Color.black;
    [Range(1, 15)][SerializeField] private int outlineThickness = 2;

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
        _outlineMaterial.SetInteger("_OutlineThickness", outlineThickness);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _outlineMaterial);
    }
}
