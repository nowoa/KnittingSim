using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    private class OrbitCameraProperties
    {
        public Vector3 FocusPoint { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Zoom { get; private set; }

        public float MaxPitchAngle = 80f;

        private float _yawVelocity;
        
        public OrbitCameraProperties()
        {
            Zoom = 10f;
        }

        public void SetFocusPoint(Vector3 p)
        {
            FocusPoint = p;
        }

        public void SetYaw(float newYaw)
        {
            Yaw = newYaw;
            _yawVelocity = 0;
        }

        public void SetPitch(float newPitch)
        {
            Pitch = Mathf.Clamp(newPitch, -MaxPitchAngle, MaxPitchAngle);
        }

        public void SetZoom(float zoom)
        {
            Zoom = Mathf.Max(0.1f, zoom);
        }

        public void Update(float deltaTime)
        {
            Yaw += _yawVelocity;
            _yawVelocity = ExpoDecay(_yawVelocity, 0.0f, 15f, deltaTime);
        }

        public void AddYawForce(float force)
        {
            _yawVelocity += force;
        }

        public static float ExpoDecay(float a, float b, float decay, float deltaTime)
        {
            return b + (a - b) * Mathf.Exp(-decay * deltaTime);
        }
    }
    
    [Header("Local References")]
    [SerializeField] private Transform focus;
    [SerializeField] private Transform verticalAxis;
    [SerializeField] private Transform cameraParent;
    
    [Header("External References")]
    [SerializeField] private Camera orbitCam;
    
    // Transform Controllers
    private readonly OrbitCameraProperties _orbitProps = new();
    
    // Sensitivity Settings
    private const float _orbitAngle = 8f;
    private const float _zoomFactor = 5f;
    
    private Vector3 _smoothVelocity;

    private bool _controlsEnabled;
    private bool _useSmoothMotion = true;

    private Vector3 _lastPointPanning;
    
    private void Update()
    {
        if (_controlsEnabled)
        {
            HandleInputs();
        }
        
        _orbitProps.Update(Time.deltaTime);
        UpdateInternalTransforms();
    }

    private void HandleInputs()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            _useSmoothMotion = !_useSmoothMotion;
            Debug.Log($"OrbitCamera: Set Smooth Motion to [{_useSmoothMotion}]");
        }
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float scrolling = Input.GetAxis("Mouse ScrollWheel");

        float focusDistance = (focus.position - cameraParent.position).magnitude;

        if (Input.GetMouseButton(0))
        {
            float orbitAngle = _orbitAngle;

            if (_useSmoothMotion)
            {
                const float MATCH_YAW_WITH_PITCH = 5;
                _orbitProps.AddYawForce(mouseX * orbitAngle * MATCH_YAW_WITH_PITCH * Time.deltaTime);
            }
            else
            {
                _orbitProps.SetYaw(_orbitProps.Yaw + mouseX * orbitAngle);
            }
            
            _orbitProps.SetPitch(_orbitProps.Pitch - mouseY * orbitAngle);
        }
        
        if (Input.GetMouseButton(1))
        {
            float currentZoom = _orbitProps.Zoom;
            float dynamicZoomMultiplier = currentZoom * 0.01f;
            _orbitProps.SetZoom(currentZoom - mouseY * _zoomFactor * dynamicZoomMultiplier);
        }

        if (scrolling != 0)
        {
            float scrollFactor = scrolling < 0 ? 1.1f : 0.9f;
            _orbitProps.SetZoom(_orbitProps.Zoom * scrollFactor);
        }

        if (Input.GetMouseButton(2))
        {
            float pan = Mathf.Tan(orbitCam.fieldOfView * Mathf.Deg2Rad) * focusDistance;
            const float PAN_MULTIPLIER = 0.009f;
            pan *= PAN_MULTIPLIER;
            Vector3 goRight =  -mouseX * pan * cameraParent.right;
            Vector3 goUp =  -mouseY * pan * cameraParent.up;
            _orbitProps.SetFocusPoint(_orbitProps.FocusPoint + goRight + goUp);
        }
    }

    private static Vector3 SetDepth(Vector3 input, float depth) => new (input.x, input.y, depth);

    private void UpdateInternalTransforms()
    {
        focus.position = _orbitProps.FocusPoint;
        focus.localEulerAngles = new Vector3(0, _orbitProps.Yaw, 0);
        verticalAxis.localEulerAngles = new Vector3(_orbitProps.Pitch, 0, 0);
        cameraParent.localPosition = new Vector3(0, 0, -_orbitProps.Zoom);
        
        orbitCam.transform.position = cameraParent.position;
        orbitCam.transform.rotation = cameraParent.rotation;
    }

    #region PUBLIC API

    public void SetControlsEnabled(bool toggle)
    {
        _controlsEnabled = toggle;
    }
    
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
        _orbitProps.SetFocusPoint(targetPosition);
    }
    

    #endregion
}
