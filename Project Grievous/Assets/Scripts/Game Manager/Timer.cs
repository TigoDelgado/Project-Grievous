using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    #region Singleton
    private static Timer _instance;

    public static Timer Instance
    {
        get { return _instance; }
    }

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
    #endregion

    private bool timerIsRunning = false;

    private float startTime = -1f;

    private float currentTime;

    public float CurrentTime
    {
        get
        {
            return currentTime;
        }
    }

    private float bestTime = 0f;

    public float BestTime
    {
        get
        {
            return bestTime;
        }
    }

    private bool isBestTime => (bestTime > 0f && currentTime < bestTime) || true;

    private void Update()
    {
        if (timerIsRunning) currentTime = Time.time - startTime;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
    }

    public void StartTimer()
    {
        if (startTime == -1f) ResetTimer();
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    public void SetBestTime()
    {
        Debug.Log("is best time?");
        if (isBestTime && ScoreManager.Instance.IsHighscore)
        {
            Debug.Log("IS BEST TIME DUDE");
            bestTime = currentTime;
        }
    }
}
