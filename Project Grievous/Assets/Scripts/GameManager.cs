using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameState state;
    public static event Action<GameState> OnGameStateChanged;

    private bool isPaused = false;

    public static GameManager Instance
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

    private void Start()
    {
        UpdateGameState(GameState.Start);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Start:
                HandleStartState();
                break;
            case GameState.Running:
                HandleRunningState();
                break;
            case GameState.Paused:
                HandlePausedState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
            case GameState.Restart:
                HandleRestartState();
                break;
        }

        Debug.Log("New state: " + newState);
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleStartState()
    {
        Timer.Instance?.ResetTimer();
        UpdateGameState(GameState.Running);
    }

    private void HandleRunningState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        Timer.Instance.StartTimer();
    }

    private void HandlePausedState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    private void HandleGameOverState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Timer.Instance.StopTimer();
    }

    private void HandleRestartState()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        UpdateGameState(GameState.Start);
    }

    public void TogglePause()
    {
        if (state == GameState.Running && !isPaused)
        {
            UpdateGameState(GameState.Paused);
            isPaused = true;
        }
        else if (state == GameState.Paused && isPaused) 
        {
            UpdateGameState(GameState.Running);
            isPaused = false;
        }
    }

    public enum GameState
    {
        Start,
        Running,
        Paused,
        GameOver,
        Restart
    }
}
