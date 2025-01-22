using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public Camera Camera;
    public Hover Hover;
    public static float GravityFactor = -0.0f;
    public static int Iterations = 2;
    public Project Project;
    public EventManager EventManager;

    private void Awake()
    {
        _instance = this;
        Camera = Camera.main;
        Hover = new Hover();
        EventManager = new EventManager();
        Project = new Project();
    }

    private void Start()
    {
    }

    private void OnDrawGizmos()
    {
        if (Project == null) return;
        Project.Simulator.DrawGizmos(Color.white);
        if (Hover.HoveredStitch==null) return;
        Gizmos.color = Color.black;
        Gizmos.DrawCube(Hover.HoveredStitch.Position, new Vector3(0.1f,0.1f,0.1f));
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(Hover.HoveredNode.Position,0.1f);
    }

    private void FixedUpdate()
    {
        MoveSelectedNode();
        Project?.FixedUpdate(Iterations,Time.fixedDeltaTime);
        CheckBoundingBox();
    }

    private void MoveSelectedNode()
    {
        if (Hover.SelectedNode == null) return;
        Hover.SelectedNode.Position = Hover.SelectedNode.AnchoredPosition = Hover.GetMouseWorldPos();
        Debug.Log(Hover.GetMouseWorldPos());
    }

    private void Simulate()
    {
        if (Project == null) return;
        Project.Simulator.Simulate(Iterations,Time.fixedDeltaTime);
    }

    private void AnchorNodes()
    {
        Project.AnchorNodes();
    }

    private void UpdateProject()
    {
        Project.UpdatePanelPosition();
        Project.UpdateMeshPosition();
    }

    private void OnGUI()
    {
        foreach (var p in Project.GetPanels())
        {
            var BBValue = Hover.IMGUIBoundingBox(p);
            Rect boundingBox = Rect.MinMaxRect(BBValue.Min.x,BBValue.Min.y,BBValue.Max.x,BBValue.Max.y);
            GUI.Box(boundingBox, p.Name+ " bounding box");/*
            Rect test = Rect.MinMaxRect(0, 0, Screen.width, Screen.height);
            GUI.Box(test, "test");*/
        }
    }

    private void CheckBoundingBox()
    {
        Hover.UpdateHover(Project.GetPanels());
    }
}
