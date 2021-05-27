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
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            UpdateGameState(GameState.MainMenu);
        } else
        {
            UpdateGameState(GameState.Start);
        }
        
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenuState();
                break;
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
            case GameState.End:
                HandleEndState();
                break;
            case GameState.Restart:
                HandleRestartState();
                break;
            case GameState.Quit:
                HandleQuitState();
                break;
        }

        Debug.Log("New state: " + newState);
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleMainMenuState()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void HandleStartState()
    {
        SceneManager.LoadScene("Level");
        isPaused = false;
        Timer.Instance?.ResetTimer();
        ScoreManager.Instance?.ResetScore();
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
        Timer.Instance?.StopTimer();
    }

    private void HandleEndState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Timer.Instance?.StopTimer();
        Timer.Instance?.SetBestTime();
        ScoreManager.Instance?.SetHighScore();
    }

    private void HandleRestartState()
    {
        UpdateGameState(GameState.Start);
    }

    private void HandleQuitState()
    {
        Application.Quit();
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
        MainMenu,
        Start,
        Running,
        Paused,
        GameOver,
        End,
        Restart,
        Quit
    }
}
