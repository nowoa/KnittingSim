using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GridMaker : MonoBehaviour
{ 

    public int width;
    [SerializeField] private int height;
    [FormerlySerializedAs("_displacementFactor")] public float displacementFactor;

    [SerializeField] private StitchScript stitchPrefab;
    [SerializeField] private GameObject _parentObject;
    private Vector3 _startingPosition;

    [SerializeField]private List<StitchScript> _stitches;

    [SerializeField] private bool isRibbed;
    [SerializeField] private int purlAmnt;
    [SerializeField] private int knitAmnt;
    
    
    
    

    [SerializeField] private Pattern _pattern;

    [SerializeField] private bool _isCircular;

    public float radius;
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
    
    public void MakeGrid()
    {
        //destroy all previous stitch gameobjects
        foreach (StitchScript i in _stitches)
        {
            GameObject.Destroy(i.gameObject);
        }
        //clear list to allow a new list to be made
        _stitches.Clear();
        
        if(_isCircular) MakeCircularGrid();
        
        else MakeSquareGrid();
        if(isRibbed) CreateRibbing();
        
    }
    
    public void MakeSquareGrid() //makes a grid of stitch gameobjects based on the width and height in the inspector
    {
        
        
        //cycle through height and width to create stitches in a grid
        for (int i = 0; i < height; i++)
        {
            Vector3 heightPos = _startingPosition + new Vector3(0, i*stitchPrefab.height, 0);
            for (int u = 0; u < width; u++)
            {
                Vector3 widthPos = _startingPosition + new Vector3(u * stitchPrefab.width, heightPos.y, 0);
                StitchScript newStitch = Instantiate(stitchPrefab, widthPos, Quaternion.identity);
                newStitch.Init(this, u, i);
                _stitches.Add(newStitch);
                if(_parentObject != null)
                {
                    newStitch.transform.parent = _parentObject.transform;
                }
            }
        }
        
        
    }
    
    public void MakeCircularGrid()
    {
        float angle = (Mathf.PI * 2) / width;
        
        float row = 0;
        float diagonalIncrement = (float)stitchPrefab.height /(float) width;
        for (int i = 0; i < height; i++)
        {
            for (int u = 0; u < width; u++)
            {
                
                Vector3 newPos = new Vector3(Mathf.Cos(angle*u)*radius, row+(float)diagonalIncrement * u, Mathf.Sin(angle*u)*radius);
            
                StitchScript newStitch = Instantiate(stitchPrefab,newPos,Quaternion.identity);
                newStitch.Init(this, u, i);
                _stitches.Add(newStitch);
                if(_parentObject != null)
                {
                    newStitch.transform.parent = _parentObject.transform;
                }
            }

            row+=stitchPrefab.height;
        }
    }
    
    public void ConnectStitches() //connects the stitches to its left and below neighbors to create references between the stitches
    {
        for (int i = 0; i < _stitches.Count; i++)
        {
            if (i % width ==0)//horizontal stitch connections
            {
                
            }
            else
            {
                _stitches[i].stitchLeft = _stitches[i - 1];
                _stitches[i - 1].stitchRight = _stitches[i];
            }

            if (i < width)//bottom stitch
            {
                
            }
            else
            {
                _stitches[i].stitchBelow = _stitches[i - width];
                _stitches[i - width].stitchAbove = _stitches[i];
            }
            if (_isCircular)
            {
                    if(i<_stitches.Count-1)
                    {
                        Debug.Log(i);
                        Debug.Log(_stitches.Count);
                        if (i%width==width-1)
                        {
                            _stitches[i].stitchRight = _stitches[i + 1];
                            _stitches[i + 1].stitchLeft = _stitches[i];
                        }
                    }
                
            }

        }
    }
    
    public void GetStitchValue()
    {
        
        
        
        
        foreach (StitchScript i in _stitches)
        {
            
            Debug.Log("is run");
            if (_pattern.GetStitch(i.xCoordinate, i.yCoordinate))
            {
                i._isKnit = true;
            }
            else i._isKnit = false;
        }
    }
   
void CreateRibbing()
{
    foreach (StitchScript i in _stitches)
    {
        if (!isRibbed)
        {
            continue;
        }

        i._isKnit = i.xCoordinate % (purlAmnt + knitAmnt) > purlAmnt - 1;
    }
}
   
    
    public void UsePattern()
    {

        width = _pattern.width;
        height = _pattern.height;
        _isCircular = _pattern.isCircular;
        radius = _pattern.radius;
        
        MakeGrid();
        Debug.Log("makegridfinished");
        ConnectStitches();
        Debug.Log("connectstitchesfinished");
        GetStitchValue();  
        
    
    }
    
}