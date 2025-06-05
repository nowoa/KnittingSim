using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NeedleRotation : MonoBehaviour
{
    public Transform Target;

    public bool isInStitch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0) || isInStitch)
        {
            transform.rotation = Quaternion.LookRotation(
                Vector3.back,
                Target.position - transform.position
            );
        }
        else
        {
            transform.localRotation =
                Quaternion.Euler(0, 0, -40);
        }
        
    }
    
}
