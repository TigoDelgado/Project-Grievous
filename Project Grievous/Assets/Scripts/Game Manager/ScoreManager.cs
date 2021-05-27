using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    #region Singleton

    private static ScoreManager _instance;

    public static ScoreManager Instance
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

    public static event Action<int> OnScoreUpdated;

    private int highScore = 0;
    public int currentScore = 0;

    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }

    public int HighScore
    {
        get
        {
            return highScore;
        }
    }

    public bool IsHighscore => currentScore > highScore;

    public void ResetScore()
    {
        currentScore = 0;
    }

    public void AddScore(int score)
    {
        currentScore += score;
        OnScoreUpdated?.Invoke(currentScore);
    }

    public bool IsHighScore()
    {
        return currentScore > highScore;
    }

    public void SetHighScore()
    {
        if (IsHighscore) highScore = currentScore;
    }
}
