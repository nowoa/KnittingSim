using System;
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
    public Transform needleTip;

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

    private Vector3 needleTargetPos;
    private Vector3 currentNeedlePosition;
    private float needleCorrectionSpeed = 10f;

    private enum State
    {
        ENTER_STITCH,
        WRAP_YARN,
        GRAB_NEEDLE,
        ADVANCE_STITCHES
    }

    private State _currentState = State.ENTER_STITCH;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentNeedlePosition = RightNeedle.transform.localPosition;
        needleTargetPos = new Vector3(3.75f, 6.62f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MoveArms();
        StateUpdate();
    }

    private void StateUpdate()
    {
        RightNeedle.GetComponent<SpriteRenderer>().color = Color.white;
        switch (_currentState)
        {
            case State.ENTER_STITCH:
                if (Vector3.Distance(EnterStitchThreshold.position, StitchToEnterPos.position) < 0.4f && !_hasEnteredStitch)
                {
                    RightNeedle.GetComponent<SpriteRenderer>().color = Color.red;
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        EnterStitch();
                        _currentState = State.WRAP_YARN;
                    }
                }
                break;
            case State.WRAP_YARN:
                if (Vector3.Distance(GrabPositionArm.position, needleTip.position) < 1f)
                {
                    _hasWrappedYarn = true;
                    Debug.Log("yarn wrapped");
                    _currentState = State.GRAB_NEEDLE;
                }
                break;
            case State.GRAB_NEEDLE:
                if (Vector3.Distance(GrabPositionArm.position, GrabPositionNeedle.position) < 1f)
                {
                    RightNeedle.GetComponent<SpriteRenderer>().color = Color.red;
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        GrabNeedle();
                        _currentState = State.ADVANCE_STITCHES;
                    }
                }
                break;
            case State.ADVANCE_STITCHES:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AdvanceStitches();
                    _currentState = State.ENTER_STITCH;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AdvanceStitches()
    {
        
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
        needleTargetPos = new Vector3(3.75f, 6.62f, 0);
        /*RightNeedle.transform.localPosition = new Vector3(3.75f, 6.62f, 0);*/
    }

    private void MoveArms()
    {
        currentNeedlePosition = RightNeedle.transform.localPosition;
        if (!RightNeedle.GetComponent<NeedleRotation>().isInStitch)
        {
            currentNeedlePosition = Vector3.MoveTowards(
                currentNeedlePosition,
                needleTargetPos,
                Time.deltaTime * needleCorrectionSpeed
            );
            RightNeedle.transform.localPosition = currentNeedlePosition;
        }
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
    }
}
