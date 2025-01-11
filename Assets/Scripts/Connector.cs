using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class Connector //handles connecting the nodes to create a panel
{
    //a silly depiction of the positions relative to i
    // []     [BendUp]      []
    // []       [Up]    [UpRight]
    // [Left]   [i]      [Right]     [BendRight]
    // []      [Down]  [DownRight]
    
    public void ConnectNodes(Panel myPanel)
    var up = i + myWidth;
    var down = i - myWidth;
    var right = i + 1;
    var left = i - 1;
    var downRight = i - myWidth + 1;
    var upRight = i + myWidth + 1;
    var bendRight = i + 2;
    var bendUp = i + myWidth * 2;
    
    var diagonalLength = Util.CalculateDiagonal()

    var isLastInRow = (i + 1) % myWidth == 0;
    var isBeforeLastInRow = (i+2) % myWidth == 0;
    
    
}
