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

    private void Awake()
    {
        _instance = this;
        Camera = Camera.main;
        Hover = new Hover();
        EventManager = new EventManager();
        Project = new Project();
    }

    private void Start()
    {
    }
    

    private void FixedUpdate()
    {
        MoveSelectedNode();
        Project?.FixedUpdate(Iterations,Time.fixedDeltaTime);
        CheckBoundingBox();
    }

    private void MoveSelectedNode()
    {
        if (Hover.SelectedNode == null) return;
        Hover.SelectedNode.Position = Hover.SelectedNode.AnchoredPosition = Hover.GetMouseWorldPos();
        Debug.Log(Hover.GetMouseWorldPos());
    }

    private void Simulate()
    {
        if (Project == null) return;
        Project.Simulator.Simulate(Iterations,Time.fixedDeltaTime);
    }

    private void AnchorNodes()
    {
        Project.AnchorNodes();
    }

    private void UpdateProject()
    {
        Project.UpdatePanelPosition();
        Project.UpdateMeshPosition();
    }

    

    private void CheckBoundingBox()
    {
        Hover.UpdateHover(Project.GetPanels());
    }
}
