
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    
    public static bool GameInput = true; // on hovering over UI buttons should be false
    
    [SerializeField] private OrbitCamera orbitCamera;

    private bool HasProject => GameManager.Instance.Project is not null;
    

    private void Update()
    {
        orbitCamera.SetControlsEnabled(false);
        
        if(!GameInput){return;}
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            orbitCamera.SetControlsEnabled(true);
        }
        else
        {
            HandleTools();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Spacebar! {HasProject}");
            orbitCamera.Focus(GameManager.Instance.Project.FabricMesh.GetComponent<MeshRenderer>());
        }
    }

    private void HandleCamera()
    {
        
    }

    private void HandleTools()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) ToolManager.OnMainAction();

        else if (Input.GetKeyUp(KeyCode.Mouse0)) ToolManager.OnMainActionEnd();

        else if (Input.GetKeyDown(KeyCode.Mouse1)) ToolManager.OnSecondaryAction();

        else if (Input.GetKeyUp(KeyCode.Mouse1)) ToolManager.OnSecondaryActionEnd();
        
        else if (Input.GetKeyDown(KeyCode.A)) ToolManager.OnSpecialAction();
        
        else ToolManager.OnDefaultBehavior();
    }
}
