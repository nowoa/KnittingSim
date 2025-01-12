
/*
public class Decreaser : Tool
{
    private List<StitchInfo> stitchesToDecrease;
    private bool toolActivated;
    private bool rightDirection;
    private StitchInfo previousStitchInfo;
    private bool hasExecutedThisFrame;
    private List<StitchInfo> stitchesInDecrease;

    public override void DefaultBehavior()
    {
        
        base.DefaultBehavior();
        if (!toolActivated)
        {
            return;
        }

        StitchInfo stitchInfo = null;
        if (MouseHover.HoveredStitchIndex >= 0 && MouseHover.HoveredStitchIndex < FabricManager.AllStitches.Count)
        {
            stitchInfo = FabricManager.AllStitches[MouseHover.HoveredStitchIndex];
        }

        if (stitchInfo == null)
        {
            return;
        }

        if (ToolUtils.AreEqual(previousStitchInfo,stitchInfo))
        {
            return; 
        }

        hasExecutedThisFrame = false;
        previousStitchInfo = stitchInfo;

        if (stitchesToDecrease.Count == 0)
        {
            if (stitchInfo.stitchType == StitchInfo.StitchType.DecreaseFirst
                || stitchInfo.stitchType == StitchInfo.StitchType.DecreaseMiddle
                || stitchInfo.stitchType == StitchInfo.StitchType.DecreaseLast)
            {
                var stitch = stitchInfo;
                while (stitch.stitchType!=StitchInfo.StitchType.DecreaseFirst)
                {
                    stitch = stitch.StitchLeft;
                }

                stitchInfo = stitch;

                while (stitch.stitchType!=StitchInfo.StitchType.DecreaseLast)
                {
                    stitchesInDecrease.Add(stitch);
                    stitch = stitch.StitchRight;
                }
                stitchesInDecrease.Add(stitch);
            }

            if (!stitchesToDecrease.Contains(stitchInfo))
            {
                stitchesToDecrease.Add(stitchInfo);
            }
        }
        if (stitchInfo.Corners[0] == stitchesToDecrease.Last().Corners[3])
        {
            AddOrRemoveDecrease(stitchInfo, true);
        }
        
        if (stitchInfo.Corners[3] == stitchesToDecrease.Last().Corners[0])
        {
            AddOrRemoveDecrease(stitchInfo,false);
        }
    }
    
    private void AddOrRemoveDecrease(StitchInfo stitchInfo, bool isRightDirection)
    {
        if (stitchesToDecrease.Count == 1)
        {
            rightDirection = isRightDirection;
        }

        if (rightDirection && isRightDirection)
        {
            if (stitchesInDecrease.Count > 0)
            {
                foreach (var s in stitchesInDecrease)
                {
                    if (!stitchesToDecrease.Contains(s))
                    {
                        stitchesToDecrease.Add(s);
                    }
                }
                stitchesInDecrease.Clear();
            }
            if (!stitchesToDecrease.Contains(stitchInfo))
            {
                stitchesToDecrease.Add(stitchInfo);
            }

            if (stitchInfo.stitchType == StitchInfo.StitchType.DecreaseFirst)
            {
                var stitch = stitchInfo;
                while (stitch.stitchType != StitchInfo.StitchType.DecreaseLast)
                {
                    if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
                    stitch = stitch.StitchRight;
                }
                if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
            }
        }

        if (!rightDirection && !isRightDirection)
        {
            if (stitchesInDecrease.Count > 0)
            {
                for (int i = stitchesInDecrease.Count-1; i >= 0; i--)
                {
                    if(!stitchesToDecrease.Contains(stitchesInDecrease[i])) stitchesToDecrease.Add(stitchesInDecrease[i]);
                }
                stitchesInDecrease.Clear();
            }
            if (!stitchesToDecrease.Contains(stitchInfo))
            {
                stitchesToDecrease.Add(stitchInfo);
            }

            if (stitchInfo.stitchType == StitchInfo.StitchType.DecreaseLast)
            {
                var stitch = stitchInfo;
                while (stitch.stitchType != StitchInfo.StitchType.DecreaseFirst)
                {
                    if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
                    stitch = stitch.StitchLeft;
                }
                if(!stitchesToDecrease.Contains(stitch)) stitchesToDecrease.Add(stitch);
            }
        }

        if (((rightDirection && !isRightDirection)|| (!rightDirection && isRightDirection))&& !hasExecutedThisFrame)
        {
            stitchesToDecrease.Remove(stitchesToDecrease.Last());
            hasExecutedThisFrame = true;
        }
        
        Debug.Log(stitchesToDecrease.Count);
    }

    public override void MainAction()
    {
        toolActivated = true;
        stitchesToDecrease = new List<StitchInfo>();
        stitchesInDecrease = new List<StitchInfo>();
        previousStitchInfo = null;
    }

    public override void MainActionEnd()
    {
        toolActivated = false;
        if (stitchesToDecrease.Count >= 2)
        {
            var decrease = new DecreaseInfo(stitchesToDecrease.First(), stitchesToDecrease.Last(), rightDirection);
            Decrease.Main(decrease);
            FabricManager.InvokeUpdateSimulation();
            var mesh = FabricManager.AllStitches[MouseHover.HoveredStitchIndex].ParentMesh;
            if (mesh != null)
            {
                mesh.UpdateMesh();
            }
            
        }
        
    }
}*/