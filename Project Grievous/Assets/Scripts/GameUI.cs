using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] TextMeshProUGUI HealthText;
    [SerializeField] TextMeshProUGUI ScoreText;

    [SerializeField] GameObject pauseUI;
    Health m_health;

    [SerializeField]
    FirstPersonMovement player;
    [SerializeField]
    Image shieldImage;
    [SerializeField]
    Image dashImage;

    void Start()
    {
        GameManager.OnGameStateChanged += GameStateChanged;

        UpdateScore(0);
        ScoreManager.OnScoreUpdated += UpdateScore;

        pauseUI?.gameObject.SetActive(false);

        Health.onHealthUpdate += UpdateHealth;
        m_health = FindObjectOfType<Health>();
        if (m_health != null) UpdateHealth(m_health.currentHealth);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameStateChanged;
        Health.onHealthUpdate -= UpdateHealth;
    }

    void Update()
    {
        UpdateTime();

        if (player.dashes > 0)
        {
            dashImage.color = new Color(dashImage.color.r, dashImage.color.g, dashImage.color.b, 1f);
        } else
        {
            dashImage.color = new Color(dashImage.color.r, dashImage.color.g, dashImage.color.b, 0.4f);
        }

        if (player.shields > 0)
        {
            shieldImage.color = new Color(shieldImage.color.r, shieldImage.color.g, shieldImage.color.b, 1f);
        }
        else
        {
            shieldImage.color = new Color(shieldImage.color.r, shieldImage.color.g, shieldImage.color.b, 0.4f);
        }
    }

    private void UpdateTime()
    {
        float time = Timer.Instance.CurrentTime;

        string minutes = ((int)time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");

        TimerText.text = minutes + ":" + seconds;
    }

    private void UpdateScore(int score)
    {
        ScoreText.text = score.ToString();
    }

    private void UpdateHealth(float health)
    {
        HealthText.text = health.ToString();
    }

    private void GameStateChanged(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.GameOver)
        {
            pauseUI?.GetComponent<GameOverUI>().UpdateScores("Game Over");
            pauseUI?.gameObject.SetActive(true);
        }
        else if (gameState == GameManager.GameState.Paused)
        {
            pauseUI?.GetComponent<GameOverUI>().UpdateScores("Paused");
            pauseUI?.gameObject.SetActive(true);
        }
        else if (gameState == GameManager.GameState.End)
        {
            if (ScoreManager.Instance.HighScore == ScoreManager.Instance.CurrentScore) pauseUI?.GetComponent<GameOverUI>().UpdateScores("New Highscore!");
            else pauseUI?.GetComponent<GameOverUI>().UpdateScores("Keep Trying");
            pauseUI?.gameObject.SetActive(true);
        }
        else
        {
            pauseUI?.gameObject.SetActive(false);
        }
    }


    public void RestartRun()
    {
        if (GameManager.Instance.state == GameManager.GameState.GameOver || 
            GameManager.Instance.state == GameManager.GameState.Paused || 
            GameManager.Instance.state == GameManager.GameState.End)
            GameManager.Instance.UpdateGameState(GameManager.GameState.Restart);
    }

    public void QuitRun()
    {
        if (GameManager.Instance.state == GameManager.GameState.GameOver ||
            GameManager.Instance.state == GameManager.GameState.Paused ||
            GameManager.Instance.state == GameManager.GameState.End)
            GameManager.Instance.UpdateGameState(GameManager.GameState.MainMenu);
    }
}
