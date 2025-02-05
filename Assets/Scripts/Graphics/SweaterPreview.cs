using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ArgumentException = System.ArgumentException;

[RequireComponent(typeof(RawImage))]
public class SweaterPreview : MonoBehaviour
{
    private RawImage _rawImage;

    [SerializeField] private Material material;

    private Material _runtimeMaterial;
    private ComputeBuffer _pointBuffer;
    
    private void OnEnable()
    {
        _rawImage = GetComponent<RawImage>();
        _runtimeMaterial = new Material(material);
        _runtimeMaterial.name = material.name + " (Copy)";
        _rawImage.material = _runtimeMaterial;
    }
    private void OnDisable()
    {
        Destroy(_runtimeMaterial);
        if (_pointBuffer is not null)
        {
            _pointBuffer.Release();
        }
        _rawImage.material = material;
    }
    
    // the margin is relative to the UV space and should not exceed 0.5, thickness is also in UV space so small numbers should be used
    public void SetPoints(Vector2[] points, float remapMarginFactor = 0.1f, float thickness = 0.02f)
    {
        if (points.Length < 2)
        {
            throw new ArgumentException("Must have a length of at least 2", nameof(points));
        }

        if (remapMarginFactor >= 0.5)
        {
            Debug.LogWarning($"Remap Margin Factor of 0.5 or higher is not allowed", this);
            return;
        }

        if (_pointBuffer is not null)
        {
            _pointBuffer.Release();
        }

        _pointBuffer = new ComputeBuffer(points.Length, sizeof(float) * 2);

        Bounds bounds = new Bounds(points[0], Vector2.zero);
        for(int i = 1; i < points.Length; i++)
        {
            bounds.Encapsulate(points[i]);
        }
        
        Vector2 min = bounds.min;
        Vector2 max = bounds.max;
        float maxSize = Mathf.Max((max - min).x, (max - min).y);
        Vector2 leftOver = (Vector2.one * maxSize - (max - min)) * 0.5f;
        max = min + Vector2.one * maxSize;

        Vector2 marginVector = Vector2.one * remapMarginFactor;
        Vector2[] mappedPoints = points.Select(p => (p + leftOver).Remap(min, max, Vector2.zero + marginVector, Vector2.one - marginVector)).ToArray();
        
        _pointBuffer.SetData(mappedPoints);
        
        _runtimeMaterial.SetBuffer("_Points", _pointBuffer);
        _runtimeMaterial.SetInteger("_PointCount", points.Length);
        _runtimeMaterial.SetFloat("_Thickness", thickness);
    }

    public static Vector2[] PointsFromSweaterParameters(int bodyWidth, int bodyHeight, int sleeveLength, int sleeveHeight, int collarWidth, int collarHeight)
    {
        float shoulder1 = bodyWidth / 2f - collarWidth / 2f;
        float shoulder2 = bodyWidth - collarWidth - shoulder1;
        Debug.Log($"{shoulder1}, {shoulder2}");
        int lowerBodyHeight = bodyHeight - sleeveHeight;
        Vector2[] offsets =
        {
            new(0,0),
            new(bodyWidth, 0),
            new(0, lowerBodyHeight),
            new(sleeveLength, 0),
            new(0, sleeveHeight),
            new(- sleeveLength - shoulder1, 0),
            new(0, collarHeight),
            new(- collarWidth, 0),
            new(0, - collarHeight),
            new(- shoulder2 - sleeveLength, 0),
            new(0, - sleeveHeight),
            new(sleeveLength, 0),
            new(0, - lowerBodyHeight)
        };
        
        Vector2[] points = new Vector2[offsets.Length];
        Vector2 currentPoint = Vector2.zero;
        
        for (int i = 0; i < offsets.Length; i++)
        {
            currentPoint += offsets[i];
            points[i] = currentPoint;
        }

        return points;
    }

    public static Vector2[] PointsFromSweaterParameters(SweaterGenerator gen)
    {
        return PointsFromSweaterParameters(
            gen.BodyStitchWidth, 
            gen.BodyStitchHeight, 
            gen.SleeveStitchLength, 
            gen.SleeveStitchWidth, 
            gen.CollarStitchWidth, 
            gen.CollarStitchHeight);
    }

    public static int StitchCountFromSweaterParameters(int bodyWidth, int bodyHeight, int sleeveLength, int sleeveHeight, int collarWidth, int collarHeight)
    {
        int bodyPanel = bodyWidth * bodyHeight;
        int sleevePanel = sleeveLength * sleeveHeight;
        int collarPanel = collarWidth * collarHeight;
        return bodyPanel * 2 + sleevePanel * 4 + collarPanel * 2;
    }
    
    public static int StitchCountFromSweaterParameters(SweaterGenerator gen)
    {
        return StitchCountFromSweaterParameters(
            gen.BodyStitchWidth, 
            gen.BodyStitchHeight, 
            gen.SleeveStitchLength, 
            gen.SleeveStitchWidth, 
            gen.CollarStitchWidth, 
            gen.CollarStitchHeight);
    }
}
