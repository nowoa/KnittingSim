
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class NeedleManager : MonoBehaviour
{
    private KnittingGameManager kgm => KnittingGameManager.Instance;
    public Transform[] anchorPositions;
    public Transform[] stitchPositions;
    public GameObject[] stitches;
    public bool isActive;
    public List<VerletNode> _nodesOnNeedle = new List<VerletNode>();
    [HideInInspector]public int counter;
    
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
        SetStitchesVisible();
    }

    public void Advance()
    {
        ClearStitches();
        if (!isActive) PlayStitchAnimations();
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
        if (isActive) PlayStitchAnimations();
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
            /*VerletEdge.ConnectNodes(below,right, diagonalLength);
            VerletEdge.ConnectNodes(current,diagonal,diagonalLength);*/
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

    public void ClearStitches()
    {
        foreach (var s in stitchPositions)
        {
            s.gameObject.SetActive(false);
        }
    }

    public void SetAnchoredNodePositions()
    {
        for (int i = 0; i < _nodesOnNeedle.Count; i++)
        {
            _nodesOnNeedle[i].Position = anchorPositions[i].position;
        }
    }

    private void PlayStitchAnimations()
    {
        for (int i = 0; i < _nodesOnNeedle.Count-1; i++)
        {
            stitchPositions[i].gameObject.SetActive(true);
            if (i != 0)
            {
                if (isActive)
                {
                    stitchPositions[i].gameObject.GetComponent<Animator>().SetTrigger("stitchOnNeedle");
                }
                else
                {
                    stitchPositions[i].gameObject.GetComponent<Animator>().SetTrigger("stitchOffNeedle");
                }
            }
        }
        if (isActive) stitchPositions[0].gameObject.GetComponent<Animator>().SetTrigger("stitchAppear");
        else
        {
            stitchPositions[0].gameObject.GetComponent<Animator>().SetTrigger("stitchDisappear");
        }
    }

    public void SetStitchesVisible()
    {
        
        for (int i = 0; i < _nodesOnNeedle.Count-1; i++)
        {
            stitchPositions[i].gameObject.SetActive(true);
        }
    }

    public void WiggleStitches()
    {
        foreach (var stitch in stitches)
        {
            if (stitch.activeInHierarchy)
            {
                LeanTween.rotateLocal(stitch, new Vector3(0, 0, 20), 0.1f);
                LeanTween.rotateLocal(stitch, new Vector3(0, 0, -20), 0.1f).setDelay(0.1f);
                LeanTween.rotateLocal(stitch, new Vector3(0, 0, 0), 0.1f).setDelay(0.2f);
            }
        }
    }
}
