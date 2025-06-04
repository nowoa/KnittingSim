using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject RightArm;
    public GameObject LeftArm;
    public Transform RightPivot;
    public Transform LeftPivot;

    public float rotationSpeed;
    public int initialRotation;
    public float speed;
    public float armReach;
    private Vector3 storedMousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
         storedMousePosition += new Vector3(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y")* speed, 0);
         storedMousePosition = Vector3.ClampMagnitude(storedMousePosition, armReach);
         RightArm.transform.position =  storedMousePosition + RightPivot.position;
         LeftArm.transform.position = new Vector3(-storedMousePosition.x * 0.3f + LeftPivot.position.x, LeftArm.transform.position.y, 0);
         RightArm.transform.rotation = Quaternion.Euler(RightArm.transform.rotation.x,180,
             (storedMousePosition.x * rotationSpeed) + initialRotation);
         
         LeftArm.transform.rotation = Quaternion.Euler(LeftArm.transform.rotation.x, 0, (storedMousePosition.x * rotationSpeed * 0.6f));
         
         Debug.Log(storedMousePosition);
    }
}
