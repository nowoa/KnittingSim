using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDebuggerUI : MonoBehaviour
{
    private MouseDragger _mouseDragger;
    private GameObject _node;
    private GameObject _nodeUp;
    private GameObject _nodeRight;
    private GameObject _nodeDown;
    private GameObject _nodeLeft;
    private GameObject _structuralUp;
    private GameObject _structuralRight;
    private GameObject _bendUp;
    private GameObject _bendRight;
    private GameObject _shearUp;
    private GameObject _shearDown;

    public int CornerIndex;

    void Awake()
    {
        Debug.Log("awakened");
        _node = transform.Find("Node")?.gameObject;
        _nodeUp = transform.Find("NodeUp")?.gameObject;
        _nodeRight = transform.Find("NodeRight")?.gameObject;
        _nodeDown = transform.Find("NodeDown")?.gameObject;
        _nodeLeft = transform.Find("NodeLeft")?.gameObject;
        _structuralUp = transform.Find("StructuralUp")?.gameObject;
        _structuralRight = transform.Find("StructuralRight")?.gameObject;
        _bendUp = transform.Find("BendUp")?.gameObject;
        _bendRight = transform.Find("BendRight")?.gameObject;
        _shearUp = transform.Find("ShearUp")?.gameObject;
        _shearDown = transform.Find("ShearDown")?.gameObject;
        _mouseDragger = MouseDragger.Instance;
    }
    private void Start()
    {
        
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_mouseDragger.HoveredStitchIndex >= 0 && _mouseDragger.HoveredStitchIndex < FabricManager.AllStitches.Count)
        {
            var stitchInfo = FabricManager.AllStitches[_mouseDragger.HoveredStitchIndex];
            if (!stitchInfo.isInactive)
            {
                _node.GetComponent<Image>().color = stitchInfo.corners[CornerIndex] == null ? Color.red : Color.green;
                _nodeUp.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].NodeAbove == null ? Color.red : Color.green;
                _nodeRight.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].NodeRight == null ? Color.red : Color.green;
                _nodeDown.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].NodeBelow == null ? Color.red : Color.green;
                _nodeLeft.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].NodeLeft == null ? Color.red : Color.green;
                _structuralUp.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].EdgeUp == null ? Color.red : Color.green;
                _structuralRight.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].EdgeRight == null ? Color.red : Color.green;
                _shearUp.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].ShearEdgeUp == null ? Color.red : Color.green;
                _shearDown.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].ShearEdgeDown == null ? Color.red : Color.green;
                _bendUp.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].BendEdgeVertical == null ? Color.red : Color.green;
                _bendRight.GetComponent<Image>().color = stitchInfo.corners[CornerIndex].BendEdgeHorizontal == null ? Color.red : Color.green;
                
            }
        }
    }

}
