using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private static Timer _instance;

    private bool timerIsRunning = false;
    public static Timer Instance
    {
        get { return _instance; }
    }

    private float startTime;

    private float currentTime;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (timerIsRunning) currentTime = Time.time - startTime;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }
}
