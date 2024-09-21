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



    [SerializeField] private bool[,] patternGrid;
    
    [Button("initialise grid")]
    public void InitialiseGrid()
    {
        
       patternGrid= new bool[width, height];
       
    }

    public bool GetStitch(int x, int y)
    {
        /*return patternGrid[x, y];*/
        return patternGrid[x, height - 1 - y];
    }
    

    
    }

    

