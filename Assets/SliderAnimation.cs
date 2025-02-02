using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderAnimation : MonoBehaviour
{

    public GameObject sliderHandle;

    public GameObject SliderBG;

    public GameObject SliderFill;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateSliderHover()
    {
        LeanTween.scale(SliderBG, new Vector3(0.8f, 1, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(SliderFill, new Vector3(0.8f, 1, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(sliderHandle, new Vector3(2.5f, 2.5f, 1f), 0.1f).setEase(LeanTweenType.easeInOutSine);
        
    }
    public void AnimateSliderExit()
    {
        LeanTween.scale(sliderHandle, new Vector3(1.8f, 1.8f, 1f), 0.1f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(SliderBG, new Vector3(0.4f, 1, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(SliderFill, new Vector3(0.4f, 1, 1), 0.1f).setEase(LeanTweenType.easeInOutSine);
    }
}
