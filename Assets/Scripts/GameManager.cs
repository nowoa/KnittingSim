using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    /*public static Hover Hover;*/
    public static Camera camera;
    public static float GravityFactor = -0.0f;
    public static int Iterations = 2;
    public Project Project;

    private void Start()
    {
        _instance = this;
        camera = Camera.main;
        Project = new Project();
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
}
