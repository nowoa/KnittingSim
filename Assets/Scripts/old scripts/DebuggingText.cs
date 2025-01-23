using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuggingText : MonoBehaviour
{
    private float _current = 0;
    public TMP_Text fpsTextField;
    public TMP_Text size;
    private int _frameCount;
    private float _timer;
    public Slider sizeslider;

    // Update is called once per frame
    void Update()
    {
        _frameCount += 1;
        _timer += Time.deltaTime;
        var time = Mathf.Round(Time.deltaTime * 1000f *100f)/100f;
        if (_timer > 0.3f) // set to 1 for fps
        {
            _current = _frameCount;
            _frameCount = 0;
            _timer -= 0.3f;
            fpsTextField.text = "frametime: " + time.ToString() + "ms";
        }

        size.text = sizeslider.value.ToString();

    }

   
}
