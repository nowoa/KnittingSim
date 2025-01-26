using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    private class OrbitCameraProperties
    {
        public Vector3 FocusPoint { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Zoom { get; private set; }

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
            // _yawVelocity = 0;
        }

        public void SetPitch(float newPitch)
        {
            Pitch = Mathf.Clamp(newPitch, -60f, 60f);
        }

        public void SetZoom(float zoom)
        {
            Zoom = Mathf.Max(0.1f, zoom);
        }

        public void Update(float deltaTime)
        {
            Yaw += _yawVelocity;
            // _yawVelocity *= decay * deltaTime;
            _yawVelocity = ExpoDecay(_yawVelocity, 0.0f, 15f, deltaTime);
        }

        public void AddYawForce(float force)
        {
            _yawVelocity += force;
            const float MAX_VELOCITY = 2.6f;
            if (Mathf.Abs(_yawVelocity) > MAX_VELOCITY)
            {
                _yawVelocity = MAX_VELOCITY * Mathf.Sign(_yawVelocity);
            }
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
    [SerializeField] private Camera orbitCam;
    
    // Transform Controllers
    private readonly OrbitCameraProperties _orbitProps = new();
    
    // Sensitivity Settings
    private const float _orbitAngle = 1200f;
    private const float _zoomFactor = 20f;
    
    private Vector3 _smoothVelocity;
    

    private void Update()
    {
        HandleInputs();
        _orbitProps.Update(Time.deltaTime);
        
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

        const int MAGIC_PIXEL_OFFSET = 30;
        Vector3 offsetPoint = orbitCam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f + MAGIC_PIXEL_OFFSET, Screen.height * 0.5f + MAGIC_PIXEL_OFFSET, _orbitProps.Zoom));
        Vector3 panDifference = offsetPoint - _orbitProps.FocusPoint;
        float panX = Vector3.Dot(panDifference, cameraParent.right);
        float panY = Vector3.Dot(panDifference, cameraParent.up);

        if (Input.GetMouseButton(0))
        {
            float orbitAngle = _orbitAngle * Time.deltaTime;
            
            _orbitProps.AddYawForce(mouseX * orbitAngle * 0.1f);
            _orbitProps.SetPitch(_orbitProps.Pitch - mouseY * orbitAngle);
        }
        
        if (Input.GetMouseButton(1))
        {
            float currentZoom = _orbitProps.Zoom;
            _orbitProps.SetZoom(currentZoom - mouseY * Time.deltaTime * currentZoom * _zoomFactor);
        }

        if (scrolling != 0)
        {
            float scrollFactor = scrolling > 0 ? 1.1f : 0.9f;
            Debug.Log(scrollFactor);
            _orbitProps.SetZoom(_orbitProps.Zoom * scrollFactor);
        }

        if (Input.GetMouseButton(2))
        {

            Vector3 panRight = panX * -mouseX * cameraParent.right;
            Vector3 panUp = panY * -mouseY * cameraParent.up;
            _orbitProps.SetFocusPoint(_orbitProps.FocusPoint + panRight + panUp);
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
        _orbitProps.SetFocusPoint(targetPosition);
    }
    

    #endregion
}
