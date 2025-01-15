using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public Camera Camera;
    public static Hover Hover;
    public static float GravityFactor = -0.0f;
    public static int Iterations = 2;
    public Project Project;

    private void Awake()
    {
        _instance = this;
        Camera = Camera.main;
        Hover = new Hover();
        Project = new Project();
    }

    private void Start()
    {
    }

    private void OnDrawGizmos()
    {
        if (Project == null) return;
        Project.Simulator.DrawGizmos(Color.white);
    }

    private void FixedUpdate()
    {
        Simulate();
        UpdateProject();
    }

    private void Simulate()
    {
        if (Project == null) return;
        Project.Simulator.Simulate(Iterations,Time.fixedDeltaTime);
    }

    private void UpdateProject()
    {
        Project.UpdatePanels();
    }

    private void OnGUI()
    {
        
        foreach (var p in Project.GetPanels())
        {
            var BBValue = Hover.PanelBoundingBox(p);
            Rect boundingBox = Rect.MinMaxRect(BBValue.xMin,BBValue.yMin,BBValue.xMax,BBValue.yMax);
            GUI.Box(boundingBox, "bounding box");
        }
    }
}
