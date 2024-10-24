using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class Pattern : SerializedScriptableObject
{

    public int width;
    public int height;
    public bool isCircular;
    [EnableIf("isCircular")]
    public float radius;



    [SerializeField] private bool[,] _patternGrid;
    
    [Button("initialise grid")]
    public void InitialiseGrid()
    {
        
       _patternGrid= new bool[width, height];
       
    }

    public bool GetStitch(int x, int y)
    {
        return _patternGrid[x, height - 1 - y];
    }
    

    
    }

    

