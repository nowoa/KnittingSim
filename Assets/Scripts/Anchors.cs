
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
        _anchors.Add(node, new Anchor(position));
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
    public GameObject PinModel;

    public Anchor(Vector3 position)
    {
        Position = position;
        PinModel = GameManager.Instance.Visualisers.CreateVisualiser(GameManager.Instance.Visualisers.PinNeedlePrefab);
        var renderer = PinModel.GetComponentInChildren<MeshRenderer>();
        var color = Random.ColorHSV(0f, 1f, 0.5f, 0.8f, 0.8f, 1f);
        MaterialPropertyBlock mpb = new();
        mpb.SetColor("_Color", color);
        renderer.SetPropertyBlock(mpb);
    }

    public void UpdatePinModelPosition(VerletNode node)
    {
        PinModel.transform.position = node.Position;
        PinModel.transform.rotation = Quaternion.LookRotation(node.Normal);
    }

    public void Cleanup()
    {
        GameManager.Instance.Visualisers.DestroyVisualiser(PinModel);
    }
}