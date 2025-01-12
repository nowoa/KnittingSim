using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public static float GravityFactor = -0.0f;
    public static int Iterations = 2;
    public Project Project;

    private void Start()
    {
        _instance = this;
        Project = new Project();
    }

    private void OnDrawGizmos()
    {
        if (Project == null) return;
        if (Project.Simulator==null) return;
        Project.Simulator.DrawGizmos(Color.white);
    }

    private void FixedUpdate()
    {
        if (Project == null) return;
        if (Project.Simulator == null) return;
        Project.Simulator.Simulate(Iterations,Time.fixedDeltaTime);
    }
}
