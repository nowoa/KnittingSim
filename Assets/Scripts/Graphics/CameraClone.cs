using UnityEngine;
using UnityEngine.Events;


public class CameraClone : MonoBehaviour
{
    [SerializeField] private Camera followCam;
    [SerializeField] private LayerMask cullingMask;
    [Range(0, 10)][SerializeField] private int lodLevel = 0;
    
    private Camera _selfCam;

    private Transform _followTransform => followCam.transform;

    private RenderTexture _renderTexture;
    public RenderTexture RenderTexture => _renderTexture;
    
    private int _screenWidth;
    private int _screenHeight;

    [HideInInspector] public UnityEvent<RenderTexture> onUpdateTexture = new();
    
    private void Awake()
    {
        _selfCam = gameObject.AddComponent<Camera>();
        _selfCam.backgroundColor = Color.black;
        _selfCam.clearFlags = CameraClearFlags.SolidColor;
        _selfCam.cullingMask = cullingMask.value;
        
        UpdateRenderTexture();
    }

    private void Start()
    {
        UpdateRenderTexture();
    }

    private void UpdateRenderTexture()
    {
        if (_screenWidth == Screen.width || _screenHeight == Screen.height)
        {
            return;
        }

        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        if (_renderTexture is not null)
        {
            _renderTexture.Release();
            _renderTexture = null;
        }

        int resolutionDivisor = lodLevel + 1;
        
        RenderTextureDescriptor desc = new(_screenWidth / resolutionDivisor, _screenHeight / resolutionDivisor, RenderTextureFormat.R8);
        _renderTexture = new RenderTexture(desc);
        _selfCam.targetTexture = _renderTexture;
        onUpdateTexture.Invoke(_renderTexture);
    }

    private void Update()
    {
        transform.position = _followTransform.position;
        transform.rotation = _followTransform.rotation;
        _selfCam.fieldOfView = followCam.fieldOfView;
        _selfCam.nearClipPlane = followCam.nearClipPlane;
        _selfCam.farClipPlane = followCam.farClipPlane;
    }
}
