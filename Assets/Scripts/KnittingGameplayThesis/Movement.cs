using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject RightArm;
    public GameObject LeftArm;
    public GameObject LeftNeedle;
    public GameObject RightNeedle;
    public Transform RightPivot;
    public Transform LeftPivot;
    public Transform EnterStitchThreshold;
    public Transform StitchToEnterPos;
    public Transform GrabPositionArm;
    public Transform GrabPositionNeedle;

    public int leftNeedleInitRot;
    public float rotationSpeed;
    public int initialRotation;
    public float speed;
    public float armReach;
    private Vector3 storedMousePosition;
    private float _wrapYarnTranslation;
    public float wrapYarnDelta;

    private bool _hasEnteredStitch;

    private bool _hasWrappedYarn;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        storedMousePosition += new Vector3(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed, 0);
        storedMousePosition = Vector3.ClampMagnitude(storedMousePosition, armReach);
        RightArm.transform.position = storedMousePosition + RightPivot.position;
        LeftArm.transform.position = new Vector3(-storedMousePosition.x * 0.3f + LeftPivot.position.x,
            LeftArm.transform.position.y, 0);
        RightArm.transform.rotation = Quaternion.Euler(RightArm.transform.rotation.x, 180,
            (storedMousePosition.x * rotationSpeed) + initialRotation);

        LeftArm.transform.rotation = Quaternion.Euler(LeftArm.transform.rotation.x, 0,
            (storedMousePosition.x * rotationSpeed * 0.6f));
        LeftNeedle.transform.rotation =
            Quaternion.Euler(0, 0, (-storedMousePosition.x * rotationSpeed) + leftNeedleInitRot);
        if (Vector3.Distance(EnterStitchThreshold.position, StitchToEnterPos.position) < 0.1f && !_hasEnteredStitch)
        {
            RightNeedle.GetComponent<SpriteRenderer>().color = Color.red;
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                EnterStitch();
                _hasEnteredStitch = true;
            }
            
        }
        else
        {
            RightNeedle.GetComponent<SpriteRenderer>().color = Color.white;
        }
        if (_hasEnteredStitch & !_hasWrappedYarn)
        {
            _wrapYarnTranslation += Input.GetAxis("Mouse Y");
            if (_wrapYarnTranslation >= wrapYarnDelta)
            {
                _hasWrappedYarn = true;
                _wrapYarnTranslation = 0;
                Debug.Log("yarn wrapped");
            }
        }

        if (_hasWrappedYarn)
        {
            if (Vector3.Distance(GrabPositionArm.position, GrabPositionNeedle.position) < 1f)
            {
                GrabNeedle();
            }
        }
    }

    private void EnterStitch()
    {
        // some animation & sound could go here for feedback
        RightNeedle.transform.SetParent(LeftNeedle.transform);
        RightNeedle.GetComponent<NeedleRotation>().isInStitch = true;
    }

    private void GrabNeedle()
    {
        RightNeedle.transform.SetParent(RightArm.transform);
        RightNeedle.GetComponent<NeedleRotation>().isInStitch = false;
        _hasWrappedYarn = false;
        _hasEnteredStitch = false;
        /*RightNeedle.transform.localPosition = new Vector3(3.75f, 6.62f, 0);*/
    }
}
