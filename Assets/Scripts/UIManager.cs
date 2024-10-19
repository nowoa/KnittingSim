using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verlet;

public class UIManager
{
    // Static instance for the singleton
    private static UIManager _instance;

    // Public property to access the instance
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();  // Create the singleton instance if it doesn't exist
            }
            return _instance;
        }
    }

    // Private constructor to prevent external instantiation
    private UIManager()
    {
        _screenSpacePositions = new List<Vector3>();
        _camera = Camera.main;
    }

    private List<Vector3> _screenSpacePositions;
    private Camera _camera;  // Camera should be passed or set manually
    private Vector3 _selectedStitch;

    // Method to select the closest stitch from a list of stitches
    public VerletNode ClosestStitch(List<VerletNode> myStitches)
    {
        float shortestDistance = float.MaxValue;  // Set to a large initial value
        int shortestDistanceIndex = -1;  // Initialize index
        int i = 0;

        foreach (var n in myStitches)
        {
            // Calculate distance between the mouse position and the current stitch
            float distance = Vector3.Distance(Input.mousePosition, _camera.WorldToScreenPoint(n.position));

            // Update the closest stitch if the current distance is shorter
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                shortestDistanceIndex = i;
            }

            i++;
        }

        return myStitches[shortestDistanceIndex];  // Return the closest stitch
    }

    public Vector3 MouseToWorldPos(Vector3 myMousePos)
    {
        return _camera.ScreenToWorldPoint(myMousePos);
    }
}
