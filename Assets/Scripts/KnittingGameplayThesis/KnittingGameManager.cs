using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Verlet;

public class KnittingGameManager : MonoBehaviour
{
    private static KnittingGameManager _instance;
    public static KnittingGameManager Instance => _instance;
    private bool _isTurned;
    public NeedleManager leftNeedle;
    public NeedleManager rightNeedle;
    public NeedleManager activeNeedle;
    public NeedleManager InactiveNeedle;
    public bool turnWork;
    public readonly List<VerletNode> ActiveNodes = new List<VerletNode>();
    public List<VerletNode> NodesToSimulate = new List<VerletNode>();
    private VerletSimulator _simulator;

    [FormerlySerializedAs("width")] [Range(1,8)]
    public int stitchWidth;
    [HideInInspector]public int nodeWidth;
    // Start is called before the first frame update
    void Start()
    {
        nodeWidth = stitchWidth + 1;
        _instance = this;
        activeNeedle = rightNeedle;
        InactiveNeedle = leftNeedle;
        activeNeedle.isActive = true;
        Initialise();
        _simulator = new VerletSimulator(NodesToSimulate);
    }

    private void TurnWork()
    {
        SubtractID();
        TurnID();
        activeNeedle.isActive = false;
        _isTurned = !_isTurned;
        if (_isTurned)
        {
            activeNeedle = leftNeedle;
            InactiveNeedle = rightNeedle;
        }
        else
        {
            activeNeedle = rightNeedle;
            InactiveNeedle = leftNeedle;
        }

        activeNeedle.isActive = true;
        turnWork = false;
    }

    private void SubtractID()
    {
        List<VerletNode> toRemove = new List<VerletNode>();
        foreach (var n in ActiveNodes)
        {
            n.id -= nodeWidth;
        }
    }

    private void TurnID()
    {
     activeNeedle.TurnID();   
    }

    private void Initialise()
    {
        leftNeedle.Initialise(nodeWidth);
    }

    public void AddNodeToActiveNodes(VerletNode node)
    {
        ActiveNodes.Add(node);
        if (NodesToSimulate.Count > nodeWidth * 8)
        {
            NodesToSimulate.RemoveAt(nodeWidth * 8);
        }
        NodesToSimulate.Insert(0,node);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InactiveNeedle.Advance();
            activeNeedle.Advance();
            if (turnWork)
            {
                TurnWork();
            }
            _simulator = new VerletSimulator(NodesToSimulate);
            
        }
        
        InactiveNeedle.SetAnchoredNodePositions();
        activeNeedle.SetAnchoredNodePositions();
        _simulator.Simulate(100,Time.deltaTime);
        
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        _simulator.DrawGizmos(Color.white);
    }

}
