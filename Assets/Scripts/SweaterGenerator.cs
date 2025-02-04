

using System.Linq;
using UnityEngine;
using Verlet;

public class SweaterGenerator : MonoBehaviour
{
    public int BodyStitchWidth; //TODO: when adjusting bodywidth, collarwidth should be dynamically adjusted and checked for validity
    public int BodyStitchHeight;
    public int SleeveStitchWidth;
    public int SleeveStitchLength;
    public int CollarStitchWidth; // should ALWAYS be even, since it's 2x the value put in by the user -- to make cylinder
    public int CollarStitchHeight;
    public Vector2Int Gauge;

    private Panel _bodyFront;
    private Panel _bodyBack;
    private Panel _leftSleeve;
    private Panel _rightSleeve;
    private Panel _collar;

    [ContextMenu("Generate sweater")] public void GenerateSweater()
    {
        MakePanels();
        GetPanels();
        CreateSeams();
    }

    private void MakePanels()
    {
        //TODO: set instantiate positions to something close to where the panel will be
        //body front panel
        PanelGenerator.GeneratePanel("body front", new Vector2Int(BodyStitchWidth, BodyStitchHeight), false, Gauge, new Vector3(0,0,0));
        
        //body back panel
        PanelGenerator.GeneratePanel("body back", new Vector2Int(BodyStitchWidth, BodyStitchHeight), false, Gauge, new Vector3(0,0,-4));
        
        //left sleeve
        PanelGenerator.GeneratePanel("left sleeve", new Vector2Int(SleeveStitchWidth*2, SleeveStitchLength), false, Gauge, new Vector3(-5,2,0));
        
        //right sleeve
        PanelGenerator.GeneratePanel("right sleeve", new Vector2Int(SleeveStitchWidth*2, SleeveStitchLength), false, Gauge, new Vector3(5,2,0));
        
        //collar
        PanelGenerator.GeneratePanel("collar", new Vector2Int(CollarStitchWidth*2, CollarStitchHeight), false, Gauge, new Vector3(0,10,0));
    }

    private void GetPanels()
    {
        _bodyFront = GameManager.Instance.Project.GetPanelByName("body front");
        _bodyBack = GameManager.Instance.Project.GetPanelByName("body back");
        _leftSleeve = GameManager.Instance.Project.GetPanelByName("left sleeve");
        _rightSleeve = GameManager.Instance.Project.GetPanelByName("right sleeve");
        _collar = GameManager.Instance.Project.GetPanelByName("collar");
    }

    private void CreateSeams()
    {
        // for index calculation documentation: https://imgur.com/a/iPQLGsA
        
        
        // set panel size variables

        int bodyWidth = BodyStitchWidth + 1;
        int bodyHeight = BodyStitchHeight + 1;
        int bodyNodeCount = bodyWidth * bodyHeight;

        int sleeveWidth = (SleeveStitchWidth * 2) + 1;
        int sleeveLength = SleeveStitchLength + 1;
        int sleeveNodeCount = sleeveWidth * sleeveLength;
        int halfSleeveWidth = (sleeveWidth - 1) / 2;

        int collarWidth = (CollarStitchWidth * 2) + 1;
        int collarHeight = CollarStitchHeight + 1;
        int collarNodeCount = collarWidth * collarHeight;
        int halfCollarWidth = (collarWidth - 1) / 2;

        int underArmHeight = (bodyHeight - 1) - halfSleeveWidth;
        
        // set seam length variables

        int bodySideLength = underArmHeight + 1;
        int bodyToSleeveLength = halfSleeveWidth + 1;
        int sleeveSideLength = sleeveLength;
        int shoulderLength = ((bodyWidth + 1) - ((collarWidth - 1) / 2)) / 2;
        int bodyToCollarLength = halfCollarWidth + 1;
        int collarSideLength = collarHeight;
        
        // set indices

        int body_1 = 0;
        int body_2 = bodyWidth - 1;
        int body_3 = underArmHeight * bodyWidth + bodyWidth - 1;
        int body_6 = bodyNodeCount - 1;
        int body_7 = bodyNodeCount - shoulderLength;
        int body_10 = bodyNodeCount - bodyWidth + shoulderLength - 1;
        int body_11 = bodyNodeCount - bodyWidth;
        int body_14 = underArmHeight * bodyWidth;

        int sleeve_3 = sleeveNodeCount - sleeveWidth; // connects to 4_1
        int sleeve_3b = sleeveNodeCount - 1;           // connects to 4_2
        int sleeve_4 = 0;
        int sleeve_4b = sleeveWidth - 1;
        int sleeve_5 = halfSleeveWidth;
        int sleeve_6 = sleeveNodeCount - 1 - halfSleeveWidth;

        int sleeve_11 = sleeve_6;
        int sleeve_12 = sleeve_5;
        int sleeve_13 = sleeve_4b;
        int sleeve_13b = sleeve_4;
        int sleeve_14 = sleeve_3b;
        int sleeve_14b = sleeve_3;
        

        int collar_7 = (collarWidth - 1 ) / 2;
        int collar_8 = collarNodeCount - 1 - halfCollarWidth;
        int collar_9 = collarNodeCount - collarWidth; //connects to 10_1
        int collar_9b = collarNodeCount - 1;           //connects to 10_2
        int collar_10 = 0;
        int collar_10b = collarWidth - 1;

        // get seam nodes

        
        //body side seam left
        var bodySideSeamLeft_front = GetSeamNodes(body_1, bodySideLength, _bodyFront, false);
        var bodySideSeamLeft_back = GetSeamNodes(body_2, bodySideLength, _bodyBack, false);
        Seam.ConnectSeams(bodySideSeamLeft_front, bodySideSeamLeft_back);
        //body side seam right
        var bodySideSeamRight_front = GetSeamNodes(body_2, bodySideLength, _bodyFront, false);
        var bodySideSeamRight_back = GetSeamNodes(body_1, bodySideLength, _bodyBack, false);
        Seam.ConnectSeams(bodySideSeamRight_front, bodySideSeamRight_back);
        
        
        //body to sleeve left front
        var bodyToSleeveLeft_front = GetSeamNodes(body_14, bodyToSleeveLength, _bodyFront, false);
        var sleeveToBodyLeft_front = GetSeamNodes(sleeve_14b, bodyToSleeveLength, _leftSleeve, true);
        Seam.ConnectSeams(bodyToSleeveLeft_front, sleeveToBodyLeft_front);
        //sleeve to body left back
        var bodyToSleeveLeft_back = GetSeamNodes(body_3, bodyToSleeveLength, _bodyBack, false, true);
        var sleeveToBodyLeft_back = GetSeamNodes(sleeve_11, bodyToSleeveLength, _leftSleeve, true);
        Seam.ConnectSeams(bodyToSleeveLeft_back, sleeveToBodyLeft_back);
        //body to sleeve right front
        var bodyToSleeveRight_front = GetSeamNodes(body_3, bodyToSleeveLength, _bodyFront, false);
        var sleeveToBodyRight_front = GetSeamNodes(sleeve_6, bodyToSleeveLength, _rightSleeve, true);
        Seam.ConnectSeams(bodyToSleeveRight_front, sleeveToBodyRight_front);
        //sleeve to body right back
        var bodyToSleeveRight_back = GetSeamNodes(body_14, bodyToSleeveLength, _bodyBack, false, true);
        var sleeveToBodyRight_back = GetSeamNodes(sleeve_3, bodyToSleeveLength, _rightSleeve, true);
        Seam.ConnectSeams(bodyToSleeveRight_back, sleeveToBodyRight_back);
        
        
        //sleeve side seam left
        var sleeveSideSeamLeft_front = GetSeamNodes(sleeve_13, sleeveSideLength, _leftSleeve, false);
        var sleeveSideSeamLeft_back = GetSeamNodes(sleeve_13b, sleeveSideLength, _leftSleeve, false);
        Seam.ConnectSeams(sleeveSideSeamLeft_front, sleeveSideSeamLeft_back);
        //sleeve side seam right
        var sleeveSideSeamRight_front = GetSeamNodes(sleeve_4, sleeveSideLength, _rightSleeve, false);
        var sleeveSideSeamRight_back = GetSeamNodes(sleeve_4b, sleeveSideLength, _rightSleeve, false);
        Seam.ConnectSeams(sleeveSideSeamRight_front, sleeveSideSeamRight_back);
        
        
        //shoulder seam left
        var shoulderSeamLeft_front = GetSeamNodes(body_11, shoulderLength, _bodyFront, true);
        var shoulderSeamLeft_back = GetSeamNodes(body_7, shoulderLength, _bodyBack, true, true);
        Seam.ConnectSeams(shoulderSeamLeft_front, shoulderSeamLeft_back);
        //shoulder seam right
        var shoulderSeamRight_front = GetSeamNodes(body_7, shoulderLength, _bodyFront, true);
        var shoulderSeamRight_back = GetSeamNodes(body_11, shoulderLength, _bodyBack, true, true);
        Seam.ConnectSeams(shoulderSeamRight_front, shoulderSeamRight_back);
        
        
        //body to collar front
        var bodyToCollarSeam_front = GetSeamNodes(body_10, bodyToCollarLength, _bodyFront, true);
        var collarToBodySeam_front = GetSeamNodes(collar_10, bodyToCollarLength, _collar, true);
        Seam.ConnectSeams(bodyToCollarSeam_front, collarToBodySeam_front);
        
        
        //body to collar back
        var bodyToCollarSeam_back = GetSeamNodes(body_10, bodyToCollarLength, _bodyBack, true);
        var collarToBodySeam_back = GetSeamNodes(collar_7, bodyToCollarLength, _collar, true);
        Seam.ConnectSeams(bodyToCollarSeam_back, collarToBodySeam_back);
        
        
        //collar side seam
        var collarSideSeam_front = GetSeamNodes(collar_10, collarSideLength, _collar, false);
        var collarSideSeam_back = GetSeamNodes(collar_10b, collarSideLength, _collar, false);
        Seam.ConnectSeams(collarSideSeam_front, collarSideSeam_back);

        

    }

    private VerletNode[] GetSeamNodes(int startIndex, int length, Panel panel, bool horizontal, bool reverse = false)
    {
        int step = horizontal ? 1 : panel.Width;
        int[] indices = new int[length];
        VerletNode[] result = new VerletNode[length];

        for (int i = 0; i < length; i++)
        {
            indices[i] = startIndex + i * step;
        }

        if (reverse) indices = indices.Reverse().ToArray();

        for (var index = 0; index < indices.Length; index++)
        {
            result[index] = panel.Nodes[indices[index]];
        }

        return result;
    }
    
    
    
    
}
