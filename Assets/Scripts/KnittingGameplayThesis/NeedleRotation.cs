using UnityEngine;

public class NeedleRotation : MonoBehaviour
{
    public Transform Target;

    public bool isInStitch;

    private Quaternion targetRotation;

    private Quaternion currentRotation;

    public float rotationSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        currentRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Update targetRotation based on mode
        if (Input.GetKey(KeyCode.Mouse0) || isInStitch)
        {
            // Aiming mode (world space)
            targetRotation = Quaternion.LookRotation(
                Vector3.back, 
                Target.position - transform.position
            );
        }
        else
        {
            // Non-aiming mode (local rotation offset)
            targetRotation = transform.parent.rotation * Quaternion.Euler(0, 0, -40);
        }

        // Smoothly interpolate rotation
        currentRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );

        // Apply smoothed rotation
        transform.rotation = currentRotation;
    }
    
}
