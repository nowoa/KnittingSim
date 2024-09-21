using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GridMaker : MonoBehaviour
{ 

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float _displacementFactor;

    [SerializeField] private StitchScript stitchPrefab;
    [SerializeField] private GameObject _parentObject;
    private Vector3 _startingPosition;

    [SerializeField]private List<StitchScript> _stitches;

    [SerializeField] private Pattern _pattern;
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
        /*for (int i=0; i<_stitches.Count;i++)
        {
            
            if (stitchPrefab.stitchLeft == null)
            {

            }
            else
            {
                _stitches[i].leftPos = new Vector3(_stitches[i].leftPos.x, _stitches[i].leftPos.y,
                    _stitches[i].GetDepth() + _stitches[i].stitchLeft.GetDepth() / 2);
            }
        }*/
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
            Vector3 heightPos = _startingPosition + new Vector3(0, i*stitchPrefab.height, 0);
            for (int u = 0; u < width; u++)
            {
                Vector3 widthPos = _startingPosition + new Vector3(u * stitchPrefab.width, heightPos.y, 0);
                StitchScript newStitch = Instantiate(stitchPrefab, widthPos, Quaternion.identity);
                newStitch.Init();
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
    
    
    public void Coordinate()
    {
        for (int i=0; i <_stitches.Count;i++)
        {

            var coordinate = GetCoordinate(i, width);
            _stitches[i].xCoordinate = coordinate.x ;
            _stitches[i].yCoordinate = coordinate.y;

        }
    }

    public Vector2Int GetCoordinate (int index, int width)
    {
        return new Vector2Int(index % width, index / width);
    }
    
    public void UsePattern()
    {

        width = _pattern.width;
        height = _pattern.height;
        MakeGrid();
        ConnectStitches();
        Coordinate();
        foreach (StitchScript i in _stitches)
        {
            if (_pattern.GetStitch(i.xCoordinate, i.yCoordinate))
            {
                i._isKnit = true;
            }
            else i._isKnit = false;
        }

        ApplyDisplacement();
    }
    
    IEnumerator CallApplyDisplacement()
    {
        yield return null; // Wait for the next frame
        ApplyDisplacement();
    }
    public void ApplyDisplacement() //applies displacement based on whether the stitch is a knit (1) or purl (0) 
    {
        foreach (StitchScript i in _stitches)
        {
           
            i.leftPos.z = i.centerPos.z = i.rightPos.z = i.GetDepth() *_displacementFactor;
            Debug.Log(i.leftPos.z, i);
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
