using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static bool GameInput = true;
    private void Start()
    {
        
    }

    private void Update()
    {
        if(!GameInput){return;}
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ToolManager.OnMainAction();
        }

        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ToolManager.OnMainActionEnd();
        }

        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToolManager.OnSecondaryAction();
        }

        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            ToolManager.OnSecondaryActionEnd();
        }
        
        else if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            //middle mouse
        }

        else
        {
            ToolManager.OnDefaultBehavior();
        }
        
    }
}
