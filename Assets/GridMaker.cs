using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GridMaker : MonoBehaviour
{ 

    [SerializeField] private int width;
    [SerializeField] private int height;

    [FormerlySerializedAs("stitch")] [FormerlySerializedAs("stitchSize")] [SerializeField] private StitchScript stitchScript;
    [SerializeField] private GameObject _parentObject;
    private Vector3 _startingPosition;

    [SerializeField]private List<StitchScript> _stitches;
    
    
    // Start is called before the first frame update
    void Start()
    {

        _stitches = new List<StitchScript>();
        _startingPosition = new Vector3(0, 0, 0);
    }

    private void OnValidate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i<_stitches.Count;i++)
        {
            
            if (stitchScript.stitchLeft == null)
            {

            }
            else
            {
                _stitches[i].leftPos = new Vector3(_stitches[i].leftPos.x, _stitches[i].leftPos.y,
                    _stitches[i].depth + _stitches[i].stitchLeft.depth / 2);
            }
        }
    }
    
    public void MakeGrid() //makes a grid of stitch gameobjects based on the width and height in the inspector
    {
        //destroy all previous stitch gameobjects
        foreach (StitchScript i in _stitches)
        {
            GameObject.Destroy(i.gameObject);
        }
        //clear list to allow a new list to be made
        _stitches.Clear();
        
        //cycle through height and width to create stitches in a grid
        for (int i = 0; i < height; i++)
        {
            Vector3 heightPos = _startingPosition + new Vector3(0, i*stitchScript.height, 0);
            for (int u = 0; u < width; u++)
            {
                Vector3 widthPos = _startingPosition + new Vector3(u * stitchScript.width, heightPos.y, 0);
                StitchScript newStitch = Instantiate(stitchScript, widthPos, Quaternion.identity);
                _stitches.Add(newStitch);
                if(_parentObject != null)
                {
                    newStitch.transform.parent = _parentObject.transform;
                }
                }
        }

        
    }
    
    public void ConnectStitches() //connects the stitches to its left and below neighbors to create references between the stitches
    {
        for (int i = 0; i < _stitches.Count; i++)
        {
            if (i % width ==0)
            {
                Debug.Log("no left stitch" + i);
                
            }
            else
            {
                _stitches[i].stitchLeft = _stitches[i - 1];
            }

            if (i < width)
            {
                
            }
            else
            {
                _stitches[i].stitchBelow = _stitches[i - width];
            }
        }
    }
    
    public void ApplyDisplacement() //applies displacement based on whether the stitch is a knit (1) or purl (0) 
    {
        foreach (StitchScript i in _stitches)
        {

            i.leftPos.z = i.centerPos.z = i.rightPos.z = i.depth;
            
        }
        foreach (StitchScript i in _stitches)
        {

            if (i.stitchLeft != null)
            {
                float depth = (i.leftPos.z + i.stitchLeft.rightPos.z) / 2;
                /*Debug.Log(i.leftPos.z.ToString() + i.stitchLeft.rightPos.z.ToString());*/
                i.leftPos.z = i.stitchLeft.rightPos.z = depth;
            }
            
        }
    }
    
   
}
