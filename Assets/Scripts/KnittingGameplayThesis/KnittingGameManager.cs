using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class KnittingGameManager : MonoBehaviour
{
    private static KnittingGameManager _instance;
    public static KnittingGameManager Instance => _instance;
    private bool _isTurned;
    public NeedleManager leftNeedle;
    public NeedleManager rightNeedle;
    public NeedleManager activeNeedle;

    public List<VerletNode> nodes = new List<VerletNode>();

    public int width;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        activeNeedle = rightNeedle;
        Initialise();
    }

    public void TurnWork()
    {
        activeNeedle.isActive = false;
        _isTurned = !_isTurned;
        if (_isTurned)
        {
            activeNeedle = leftNeedle;
        }
        else
        {
            activeNeedle = rightNeedle;
        }

        activeNeedle.isActive = true;
    }

    private void Initialise()
    {
        for (int i = 0; i < 4; i++)
        {
            var node = new VerletNode(leftNeedle.anchorPositions[i].position)
            {
                id = i
            };
            nodes.Add(node);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
