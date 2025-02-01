
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Verlet;

public class Anchors
{
    private Dictionary<VerletNode, Anchor> _anchors = new();


    public void ToggleAnchor(VerletNode node, Vector3 position)
    {
        if (_anchors.ContainsKey(node))
        {
            RemoveAnchor(node);
        }
        else
        {
            CreateAnchor(node, position);
        }
    }

    public void MoveAnchor(VerletNode node)
    {
        _anchors[node].Position = GameManager.Instance.Hover.GetMouseWorldPos();
    }
    private void CreateAnchor(VerletNode node, Vector3 position)
    {
        _anchors.Add(node, new Anchor(position, node.Normal));
    }

    private void RemoveAnchor(VerletNode node)
    {
        var item = _anchors[node];
        item.Cleanup();
        _anchors.Remove(node);
    }

    public void UpdateNodePositions()
    {
        foreach (var (node, anchor) in _anchors)
        {
            node.Position = anchor.Position;
        }
    }

    public void UpdatePinPositions()
    {
        foreach (var (node, anchor) in _anchors)
        {
            anchor.UpdatePinModelPosition(node);
        }
    }

    public VerletNode[] AnchoredNodes()
    {
        return _anchors.Keys.ToArray();
    }

    public Dictionary<VerletNode, Anchor> GetAnchors()
    {
        return _anchors;
    }
}

public class Anchor
{
    public Vector3 Position;
    private readonly GameObject _pinModel;

    private readonly MeshRenderer _pinRenderer;
    private MaterialPropertyBlock _mpb;

    private static int _colorProp = Shader.PropertyToID("_Color");
    private static int _highlightProp = Shader.PropertyToID("_Highlight");

    public Anchor(Vector3 position, Vector3 normal, float highlight = 0.0f)
    {
        Position = position;
        _pinModel = GameManager.Instance.Visualisers.CreateVisualiser(GameManager.Instance.Visualisers.PinNeedlePrefab, position, Quaternion.LookRotation(normal));
        _pinRenderer = _pinModel.GetComponentInChildren<MeshRenderer>();
        var color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f);
        _mpb = new();
        _mpb.SetColor(_colorProp, color);
        _mpb.SetFloat(_highlightProp, highlight);
        _pinRenderer.SetPropertyBlock(_mpb);
    }

    public void SetHighlight(float highlight)
    {//value between 0-1
        _pinRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_highlightProp, highlight);
        _pinRenderer.SetPropertyBlock(_mpb);
    }

    public void UpdatePinModelPosition(VerletNode node)
    {
        _pinModel.transform.position = node.Position;
        _pinModel.transform.rotation = Quaternion.Slerp(_pinModel.transform.rotation,Quaternion.LookRotation(node.Normal), 0.2f);
    }

    public void Cleanup()
    {
        GameManager.Instance.Visualisers.DestroyVisualiser(_pinModel);
    }
}