using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float cameraSpeed;
    public static bool GameInput = true;
    private Transform _cameraPos;
    private void Start()
    {
        _cameraPos = Camera.main.transform;
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

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _cameraPos.position += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetKey(KeyCode.A))
        {
            _cameraPos.position += new Vector3(-cameraSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _cameraPos.position += new Vector3(cameraSpeed * Time.deltaTime, 0, 0);
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            _cameraPos.position += new Vector3(0, cameraSpeed *Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.S))
        {
            _cameraPos.position += new Vector3(0, -cameraSpeed *Time.deltaTime, 0);
        }
        
    }
}
