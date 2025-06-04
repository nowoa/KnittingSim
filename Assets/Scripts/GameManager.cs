using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    public Camera Camera;
    public Hover Hover;
    public static float GravityFactor = -0.0f;
    public static int Iterations = 2;
    public Project Project;
    public EventManager EventManager;
    public Visualisers Visualisers;
    public ColorPalette ColorPalette;

    public MeshFilter detailMeshFilter;

    private void Awake()
    {
        _instance = this;
        Camera = Camera.main;
        Hover = new Hover();
        EventManager = new EventManager();
    }

    private void Start()
    {
        Project = new Project();
        OrbitCamera.Instance.SetTransforms(0.01f,0,20);
    }


    private void FixedUpdate()
    {
        if (Project is null) return;
        if (Project.GetPanels().Count == 0) return;
        
        MoveSelectedNode();
        
        Project.FixedUpdatePreHover(Iterations,Time.fixedDeltaTime);
        
        using (new ProfileSample("Update Hover"))
            Hover.UpdateHover(Project.HashGridScreen, Project.StitchScreenPositions, Project.Stitches);
        
        Project.FixedUpdatePostHover();
    }

    private void MoveSelectedNode()
    {
        if (Hover.SelectedNode == null) return;
        if (Project.anchors.GetAnchors().ContainsKey(Hover.SelectedNode))
        {
            Project.anchors.MoveAnchor(Hover.SelectedNode);
            return;
        }

        Hover.SelectedNode.Position = Hover.GetMouseWorldPos();

    }
}
