using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public GarmentGenerator GarmentGenerator;
    private int _nodeCount;
    private float _current = 0;
    public TMP_Text textField;
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
            textField.text = "fps: " + _current.ToString() + " nodes: " + _nodeCount.ToString();
            
        }
        //_current = (int)(1f / Time.unscaledDeltaTime);
    }

   
}
