using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DebuggingText : MonoBehaviour
{
    public GarmentGenerator GarmentGenerator;
    private int _nodeCount;
    private float _current = 0;
    public TMP_Text fpsTextField;
    public TMP_Text stitchTypeTextField;
 public TMP_Text knitValueTextField;
    private int _frameCount;

    private float _timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _nodeCount = FabricManager.NodeCount;
        _frameCount += 1;
        _timer += Time.deltaTime;
        if (_timer > 1)
        {
            _current = _frameCount;
            _frameCount = 0;
            _timer -= 1;
            fpsTextField.text = "fps: " + _current.ToString() + " nodes: " + _nodeCount.ToString();
            
        }

        if (MouseDragger.Instance.HoveredStitchIndex >= 0 &&
            MouseDragger.Instance.HoveredStitchIndex < FabricManager.AllStitches.Count)
        {
            stitchTypeTextField.text =
                FabricManager.AllStitches[MouseDragger.Instance.HoveredStitchIndex].stitchType.ToString();
        }
        if (MouseDragger.Instance.HoveredStitchIndex >= 0 &&
            MouseDragger.Instance.HoveredStitchIndex < FabricManager.AllStitches.Count)
        {
            knitValueTextField.text =
                FabricManager.AllStitches[MouseDragger.Instance.HoveredStitchIndex].Knit ? "knit" : "purl";
        }
    }

   
}
