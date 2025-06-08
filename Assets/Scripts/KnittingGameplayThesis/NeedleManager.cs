
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class NeedleManager : MonoBehaviour
{
    private KnittingGameManager kgm => KnittingGameManager.Instance;
    public Transform[] anchorPositions;
    public Transform[] stitchPositions;
    public Transform positionA;
    public Transform positionB;
    public bool isActive;
    private List<VerletNode> _nodesOnNeedle = new List<VerletNode>();
    public int counter;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var s in stitchPositions)
        {
            s.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialise(int width)
    {
        for (int i = 0; i < width; i++)
        {
            var node = new VerletNode(anchorPositions[i].position)
            {
                id = i
            };
            _nodesOnNeedle.Add(node);
            kgm.AddNodeToActiveNodes(node);
        }
    }

    public void Advance()
    {
        foreach (var s in stitchPositions)
        {
            s.gameObject.SetActive(false);
        }
        
        if (isActive)
        {
            if (_nodesOnNeedle.Count == 0)
            {
                var extraNode = new VerletNode(anchorPositions[1].position)
                {
                    id = counter + kgm.nodeWidth
                };
                
                _nodesOnNeedle.Insert(0, extraNode);
                kgm.AddNodeToActiveNodes(extraNode);
                MakeStitch(counter+kgm.nodeWidth);
                counter++;
            }
            var node = new VerletNode(anchorPositions[0].position)
            {
                id = counter + kgm.nodeWidth
            };
            _nodesOnNeedle.Insert(0, node);
            kgm.AddNodeToActiveNodes(node);
            MakeStitch(counter+kgm.nodeWidth);
            counter++;
            
        }
        else
        {
            counter = 0;
            if (_nodesOnNeedle.Count >= 1)
            {
                _nodesOnNeedle.RemoveAt(0);
            }

            if (_nodesOnNeedle.Count == 1)
            {
                _nodesOnNeedle.RemoveAt(0);
            }
            if (_nodesOnNeedle.Count == 0)
            {
                kgm.turnWork = true;
            }
        }
        SetAnchoredNodePositions();
        
    }

    private void MakeStitch(int id)
    {
        VerletNode current = kgm.ActiveNodes.Find(node => node.id == id);
        
        VerletNode below = kgm.ActiveNodes.Find(node => node.id == id - kgm.nodeWidth);

        float width = 1.3f;
        float length = 0.6f;
        float diagonalLength = Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(length, 2));
        if (id - 1 - kgm.nodeWidth >= 0)
        {
            VerletNode diagonal = kgm.ActiveNodes.Find(node => node.id == id - 1 - kgm.nodeWidth);
            VerletNode right = kgm.ActiveNodes.Find(node => node.id == id - 1);
            VerletEdge.ConnectNodes(below, diagonal, width);
            VerletEdge.ConnectNodes(below,right, diagonalLength);
            VerletEdge.ConnectNodes(current,diagonal,diagonalLength);
            VerletEdge.ConnectNodes(current,right,width);
            kgm.AddStitch(below,current,right,diagonal);
        }
        
        /*VerletEdge.ConnectNodes(current, right, 1f);*/
        VerletEdge.ConnectNodes(current, below, length);
        
        /*VerletEdge.ConnectNodes(right, diagonal, 1f);*/
    }

    public void TurnID()
    {
        for (int i = 0; i < _nodesOnNeedle.Count; i++)
        {
            _nodesOnNeedle[i].id = i;
        }
    }

    public void SetAnchoredNodePositions()
    {
        for (int i = 0; i < _nodesOnNeedle.Count; i++)
        {
            _nodesOnNeedle[i].Position = anchorPositions[i].position;
            
            
        }
        for (int i = 0; i < _nodesOnNeedle.Count-1; i++)
        {
            stitchPositions[i].gameObject.SetActive(true);
            
            
        }
    }
}
