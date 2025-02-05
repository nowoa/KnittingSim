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
    }

    public void SetPoints(Vector2[] points, float remapMarginFactor = 0.1f, float thickness = 0.02f)
    {
        if (points.Length < 2)
        {
            throw new ArgumentException("Must have a length of at least 2", nameof(points));
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
        Vector2 leftOver = Vector2.one * maxSize - (max - min);
        max = min + Vector2.one * maxSize * (1 + remapMarginFactor);
        min -= Vector2.one * remapMarginFactor;
        
        Vector2[] mappedPoints = points.Select(p => p.Remap(min, max, Vector2.zero, Vector2.one) + leftOver * 0.5f).ToArray();
        
        _pointBuffer.SetData(mappedPoints);
        
        _runtimeMaterial.SetBuffer("_Points", _pointBuffer);
        _runtimeMaterial.SetInteger("_PointCount", points.Length);
        _runtimeMaterial.SetFloat("_Thickness", thickness);
    }

    private void SetTestPoints()
    {
        // Sweater representation points
        Vector2[] points = new[]
        {
            new Vector2(0.284f, 0.085f),
            new Vector2(0.677f, 0.095f),
            new Vector2(0.664f, 0.528f),
            new Vector2(0.873f, 0.427f),
            new Vector2(0.94f, 0.563f),
            new Vector2(0.65f, 0.688f),
            new Vector2(0.641f, 0.759f),
            new Vector2(0.318f, 0.761f),
            new Vector2(0.32f, 0.697f),
            new Vector2(0.06f, 0.58f),
            new Vector2(0.149f, 0.453f),
            new Vector2(0.297f, 0.534f)
        };
        SetPoints(points);
    }
}
