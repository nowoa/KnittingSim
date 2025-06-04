using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NeedleRotation : MonoBehaviour
{
    public Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rot = Quaternion.LookRotation(
            Vector3.back,
            Target.position - transform.position
        );
        transform.rotation = rot;
    }
    
}
