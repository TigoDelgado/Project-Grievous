using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private TextMeshProUGUI HealthText;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseUI;
    Health m_health;

    void Start()
    {
        GameManager.OnGameStateChanged += GameStateChanged;

        gameOverUI?.gameObject.SetActive(false);
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
        float time = Timer.Instance.GetCurrentTime();

        string minutes = ((int) time / 60).ToString("00");
        string seconds = (time % 60).ToString("00");

        TimerText.text = minutes + ":" + seconds;
    }

    private void UpdateHealth(float health)
    {
        HealthText.text = health.ToString();
    }

    private void GameStateChanged(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.GameOver)
        {
            pauseUI?.gameObject.SetActive(false);
            gameOverUI?.gameObject.SetActive(true);
        }
        else if (gameState == GameManager.GameState.Paused)
        {
            gameOverUI?.gameObject.SetActive(false);
            pauseUI?.gameObject.SetActive(true);
        }
        else
        {
            gameOverUI?.gameObject.SetActive(false);
            pauseUI?.gameObject.SetActive(false);
        }
    }


    public void RestartRun()
    {
        if (GameManager.Instance.state == GameManager.GameState.GameOver)
            GameManager.Instance.UpdateGameState(GameManager.GameState.Restart);
    }

    public void QuitRun()
    {
        if (GameManager.Instance.state == GameManager.GameState.GameOver)
            Application.Quit();
    }
}
