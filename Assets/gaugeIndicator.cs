using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gaugeIndicator : MonoBehaviour
{

    public Slider horizontal;

    public Slider vertical;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalScale = 1 - (horizontal.value / 30);
        var verticalScale = 1 - (vertical.value / 30);
        var size = 1f;
        gameObject.transform.localScale = new Vector3(size + horizontalScale, size + verticalScale, 1);
    }
}
