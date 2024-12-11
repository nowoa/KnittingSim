using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NodeDebuggerUI : MonoBehaviour
{
    private MouseHover _mouseHover;
    private GameObject _node;
    private GameObject _structuralUp;
    private GameObject _structuralRight;
    private GameObject _bendUp;
    private GameObject _bendRight;
    private GameObject _shearUp;
    private GameObject _shearDown;
    private GameObject _stitchLeft;
    private GameObject _stitchRight;
    private GameObject _stitchAbove;
    private GameObject _stitchBelow;

    public int CornerIndex;
    public bool NodeDebugger;
    public bool StitchGridDebugger;

    void Awake()
    {
        if (NodeDebugger)
        {
            _node = transform.Find("Node")?.gameObject;
            _structuralUp = transform.Find("StructuralUp")?.gameObject;
            _structuralRight = transform.Find("StructuralRight")?.gameObject;
            _bendUp = transform.Find("BendUp")?.gameObject;
            _bendRight = transform.Find("BendRight")?.gameObject;
            _shearUp = transform.Find("ShearUp")?.gameObject;
            _shearDown = transform.Find("ShearDown")?.gameObject;
        }
        _mouseHover = MouseHover.Instance;
        if (StitchGridDebugger)
        {
            _stitchLeft = transform.Find("StitchLeft").gameObject;
            _stitchRight = transform.Find("StitchRight").gameObject;
            _stitchAbove = transform.Find("StitchAbove").gameObject;
            _stitchBelow = transform.Find("StitchBelow").gameObject;
        }
    }
    private void Start()
    {
        
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_mouseHover.HoveredStitchIndex < 0 || _mouseHover.HoveredStitchIndex >= FabricManager.AllStitches.Count)
        {
            return;
        }

        var stitchInfo = FabricManager.AllStitches[_mouseHover.HoveredStitchIndex];
        if (!stitchInfo.IsActive)
        {
            return;
        }

        if (NodeDebugger)
        {
            if (stitchInfo?.Corners[CornerIndex] == null)
            {
                _node.GetComponent<Image>().color = Color.red;
                return;
            }

            _node.GetComponent<Image>().color = Color.green;

            _structuralUp.GetComponent<Image>().color = stitchInfo.Corners[CornerIndex].EdgeUp == null ? Color.red : Color.green;
            _structuralRight.GetComponent<Image>().color = stitchInfo.Corners[CornerIndex].EdgeRight == null ? Color.red : Color.green;
            _shearUp.GetComponent<Image>().color = stitchInfo.Corners[CornerIndex].ShearEdgeUp == null ? Color.red : Color.green;
            _shearDown.GetComponent<Image>().color = stitchInfo.Corners[CornerIndex].ShearEdgeDown == null ? Color.red : Color.green;
            _bendUp.GetComponent<Image>().color = stitchInfo.Corners[CornerIndex].BendEdgeVertical == null ? Color.red : Color.green;
            _bendRight.GetComponent<Image>().color = stitchInfo.Corners[CornerIndex].BendEdgeHorizontal == null ? Color.red : Color.green;
        }

        if (StitchGridDebugger)
        {
            _stitchLeft.GetComponent<Image>().color = stitchInfo.StitchLeft == null ? Color.red : Color.green;
            _stitchRight.GetComponent<Image>().color = stitchInfo.StitchRight == null ? Color.red : Color.green;
            _stitchAbove.GetComponent<Image>().color = stitchInfo.StitchAbove == null ? Color.red : Color.green;
            _stitchBelow.GetComponent<Image>().color = stitchInfo.StitchBelow == null ? Color.red : Color.green;
        }
    }

}
