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
    }


    private void FixedUpdate()
    {
        MoveSelectedNode();
        Project?.FixedUpdate(Iterations,Time.fixedDeltaTime);
        using (new ProfileSample("Update Hover"))
            CheckBoundingBox();
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
    
    private void CheckBoundingBox()
    {
        Hover.UpdateHover(Project.ScreenHashGrid);
    }
}
