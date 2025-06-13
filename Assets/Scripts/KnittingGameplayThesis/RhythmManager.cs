using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{

    private Queue<float> _intervals = new Queue<float>(4);

    private float timeSinceLastInput;
    public Image rhythmIndicator;

    [Range(0,1)]public float rhythmBuffer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastInput += Time.deltaTime;
    }

    public void AddInterval()
    {
        if (_intervals.Count == 0)
        {
            _intervals.Enqueue(timeSinceLastInput);
            timeSinceLastInput = 0;
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
        }
        else
        {
            _intervals = new Queue<float>();
            //empty queue to start over interval calcs
            Debug.Log("incorrect");
            rhythmIndicator.color = Color.red;
        }
        timeSinceLastInput = 0f;
        
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
