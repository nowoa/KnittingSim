using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float startTime = 180f; // 3 minutes = 180 seconds
    private float currentTime;
    public TMP_Text timerText;
    public UnityEvent OnTimerEnd;

    void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(0, currentTime); // Avoid negative time
            UpdateTimerDisplay();
        }
        if (currentTime <= 0)
        {
            // Timer done
            Debug.Log("Time's up!");
            OnTimerEnd.Invoke();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}