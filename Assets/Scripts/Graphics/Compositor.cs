using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Compositor : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private Material material;
    
    private void OnEnable()
    {
        _cam = GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material is null)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        Graphics.Blit(source, destination, material);
    }
}
