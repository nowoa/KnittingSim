using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{

    private Queue<float> _intervals = new Queue<float>(4);

    private float timeSinceLastInput;
    public Image rhythmIndicator;
    private int combo;
    public TMP_Text comboText;
    private float multiplierProgress;
    public int multThreshold;
    public int multAcquireSpeed;
    private bool isAcquiringMult;
    public Slider multSlider;
    private float _inactivityTimer;
    public float inactivityThreshold;
    [HideInInspector]public int multValue;
    private bool enterMult;
    public TMP_Text multText;
        

    [Range(0,1)]public float rhythmBuffer;

    public KnittingGameSound kgs;

    public int bgmStart;
    // Start is called before the first frame update
    void Start()
    {
        multSlider.maxValue = multThreshold;
        multValue = 1;
        UpdateMultText();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastInput += Time.deltaTime;
        multiplierProgress = isAcquiringMult
            ? multiplierProgress + (multAcquireSpeed * Time.deltaTime)
            : multiplierProgress - (multAcquireSpeed * Time.deltaTime * 3f);
        multiplierProgress = Mathf.Clamp(multiplierProgress, 0, multThreshold);
        multSlider.value = multiplierProgress;
        _inactivityTimer += Time.deltaTime;

        if (_inactivityTimer>=inactivityThreshold)
        {
            isAcquiringMult = false;
            kgs.FadeOutBGM();
        }

        if (multiplierProgress >= 0.1f && !enterMult)
        {
            enterMult = true;
        }

        if (multiplierProgress >= multThreshold)
        {
            
            multValue++;
            multiplierProgress = 0;
            enterMult = false;
            UpdateMultText();
        }

        if (multiplierProgress <= 0 && enterMult)
        {
            
            if (multValue == 1)
            {
                multiplierProgress = 0;
                return;
            }
            multValue--;
            multiplierProgress = multThreshold;
            UpdateMultText();
        }
        

    }

    public void AddInterval()
    {
        _inactivityTimer = 0;
        if (_intervals.Count == 0)
        {
            _intervals.Enqueue(timeSinceLastInput);
            timeSinceLastInput = 0;
            combo = 1;
            return;
        }

        if (CheckRhythmBuffer())
        {
            if (_intervals.Count >= 4) //remove oldest item to stay at max 4 items
            {
                _intervals.Dequeue();
            }
            _intervals.Enqueue(timeSinceLastInput);
            Debug.Log("correct");
            rhythmIndicator.color = Color.green;
            combo++;
            UpdateCombo();
            isAcquiringMult = true;
        }
        else
        {
            _intervals = new Queue<float>();
            //empty queue to start over interval calcs
            Debug.Log("incorrect");
            rhythmIndicator.color = Color.red;
            combo = 0;
            UpdateCombo();
            isAcquiringMult = false;
            kgs.FadeOutBGM();
        }
        timeSinceLastInput = 0f;
        if (combo >= bgmStart)
        {
            kgs.FadeInBGM();
        }
        
    }

    private void UpdateCombo()
    {
        
        comboText.text = combo.ToString();
    }

    private void UpdateMultText()
    {
        multText.text = multValue.ToString() + "x";
    }

    private bool CheckRhythmBuffer()
    {
        float averageInterval = 0f;
        foreach (var f in _intervals)
        {
            averageInterval += f;
        }

        averageInterval /= _intervals.Count;
        Debug.Log(averageInterval);
        if (timeSinceLastInput >= averageInterval - (rhythmBuffer * averageInterval) && timeSinceLastInput <= averageInterval + (rhythmBuffer* averageInterval))
        {
            return true;
        }
        return false;

    }

}
