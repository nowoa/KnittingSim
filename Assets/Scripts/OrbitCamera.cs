using System;
using UnityEngine;


public class OrbitCamera : MonoBehaviour
{
    private class OrbitCameraProperties
    {
        public Vector3 FocusPoint { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Zoom { get; private set; }

        public void SetFocusPoint(Vector3 p)
        {
            FocusPoint = p;
        }

        public void SetYaw(float newYaw)
        {
            Yaw = newYaw;
        }

        public void SetPitch(float newPitch)
        {
            Pitch = Mathf.Clamp(newPitch, -30f, 30f);
        }

        public void SetZoom(float zoom)
        {
            Zoom = Mathf.Max(0, zoom);
        }

        public OrbitCameraProperties()
        {
            Zoom = 10f;
        }
    }
    
    [Header("Local References")]
    [SerializeField] private Transform focus;
    [SerializeField] private Transform verticalAxis;
    [SerializeField] private Transform cameraParent;
    [SerializeField] private Camera orbitCam;
    
    // Transform Controllers
    private readonly OrbitCameraProperties _orbitProps = new();
    
    // Sensitivity Settings
    private const float _orbitAngle = 1200f;
    private const float _panningDistance = 10f;
    private const float _zoomDistance = 20f;
    
    
    private const float _smoothTime = 0.01f;

    private Vector3 _followPoint;
    private Vector3 _smoothVelocity;

    private Func<Vector3> _followerFunction;
    private bool _useFollowing;

    private void OnEnable()
    {
        _followPoint = cameraParent.position;
        StopFollowing();
    }

    private void Update()
    {
        HandleInputs();

        if (_useFollowing)
        {
            Focus(_followerFunction());
        }
        
        UpdateInternalTransforms();
    }

    private void HandleInputs()
    {
        // debugging
        if (Input.GetKey(KeyCode.Space))
        {
            Focus(GameObject.Find("Monkey").GetComponent<MeshRenderer>());
        }
        
        if (!Input.GetKey(KeyCode.LeftAlt)) return;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float scrolling = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButton(0))
        {
            float orbitAngle = _orbitAngle * Time.deltaTime;
            
            _orbitProps.SetYaw(_orbitProps.Yaw + mouseX * orbitAngle);
            _orbitProps.SetPitch(_orbitProps.Pitch - mouseY * orbitAngle);
        }
        
        if (Input.GetMouseButton(1))
        {
            _orbitProps.SetZoom(_orbitProps.Zoom - mouseY * Time.deltaTime * _zoomDistance);
        }

        if (scrolling != 0)
        {
            _orbitProps.SetZoom(_orbitProps.Zoom + scrolling * _zoomDistance);
        }

        if (Input.GetMouseButton(2))
        {
            float panning = _panningDistance * Time.deltaTime;
            Vector3 direction = (cameraParent.right * -mouseX + cameraParent.up * -mouseY).normalized;
            _orbitProps.SetFocusPoint(_orbitProps.FocusPoint + direction * panning);
        }
    }

    private void UpdateInternalTransforms()
    {
        focus.position = _orbitProps.FocusPoint;
        focus.localEulerAngles = new Vector3(0, _orbitProps.Yaw, 0);
        verticalAxis.localEulerAngles = new Vector3(_orbitProps.Pitch, 0, 0);
        cameraParent.localPosition = new Vector3(0, 0, -_orbitProps.Zoom);
    }

    #region PUBLIC API

    public void Focus(MeshRenderer meshRenderer)
    {
        Bounds bounds = meshRenderer.bounds;
        Vector3 extends = bounds.extents;
        float focusRadius = Mathf.Max(extends.x, extends.y, extends.z);
        Vector3 center = bounds.center;
        
        _orbitProps.SetZoom(focusRadius * 2f);
        Focus(center);
    }

    public void Focus(Vector3 targetPosition)
    {
        if (_useFollowing)
        {
            Debug.LogWarning("Following is currently enabled. Focus call is being ignored");
            return;
        }
        _orbitProps.SetFocusPoint(targetPosition);
    }

    public void Follow(Func<Vector3> followFunction)
    {
        _followerFunction = followFunction;
        _useFollowing = true;
    }

    public void StopFollowing()
    {
        _followerFunction = null;
        _useFollowing = false;
    }

    #endregion
}
