using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
    public readonly List<SimpleStitch> Stitches = new List<SimpleStitch>();
    public readonly List<Vector3> VertexPositions = new List<Vector3>();
    public SimpleFabricMesh SimpleFabricMesh;
    private Color _color;//changes every x stitches based on colors player chose

    public Material knitMat;
    public Material purlMat;

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
        SimpleFabricMesh = GetComponent<SimpleFabricMesh>();
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
            GetComponent<MeshRenderer>().material = purlMat;
        }
        else
        {
            activeNeedle = rightNeedle;
            InactiveNeedle = leftNeedle;
            GetComponent<MeshRenderer>().material = knitMat;
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
        if (NodesToSimulate.Count > nodeWidth * 10)
        {
            NodesToSimulate.RemoveAt(nodeWidth * 10);
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
            SimpleFabricMesh.RegenerateMesh(Stitches);
            
        }
        
        InactiveNeedle.SetAnchoredNodePositions();
        activeNeedle.SetAnchoredNodePositions();
        _simulator.Simulate(100,Time.deltaTime);
        SimpleFabricMesh.UpdatePositions(GetVertexPositions());
        
    }

    private Vector3[] GetVertexPositions()
    {
        List<Vector3> result = new List<Vector3>();

        foreach (var s in Stitches)
        {
            result.AddRange(s._corners.Select(c => c.Position));
        }

        return result.ToArray();
    }

    public void AddStitch(VerletNode botLeft, VerletNode topLeft, VerletNode topRight, VerletNode botRight)
    {
        Stitches.Add(new SimpleStitch(new []{botLeft,topLeft,topRight,botRight}, _color));
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        _simulator.DrawGizmos(Color.white);
    }

    public struct SimpleStitch
    {
        public readonly VerletNode[] _corners;
        private Color _color;

        public SimpleStitch(VerletNode[] corners, Color color)
        {
            _corners = corners;
            _color = color;
        }
    }


}
