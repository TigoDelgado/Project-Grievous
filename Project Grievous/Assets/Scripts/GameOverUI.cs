using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MenuName;
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI BestTimeText;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI TimeText;

    public void UpdateScores(string menuName)
    {
        MenuName.text = menuName;

        int highScore = ScoreManager.Instance.HighScore;
        if (highScore > 0) HighScoreText.text = "Highscore: " + highScore.ToString();
        else HighScoreText.text = "Highscore: --";

        ScoreText.text = "Score: " + ScoreManager.Instance?.CurrentScore.ToString();

        string minutes, seconds;

        float bestTime = Timer.Instance.BestTime;
        if (bestTime > 0)
        {
            minutes = ((int)bestTime / 60).ToString("00");
            seconds = (bestTime % 60).ToString("00");
            BestTimeText.text = "Time: " + minutes + ":" + seconds;
        } 
        else BestTimeText.text = "Time: --:--";


        float currenTime = Timer.Instance.CurrentTime;
        minutes = ((int)currenTime / 60).ToString("00");
        seconds = (currenTime % 60).ToString("00");
        TimeText.text = "Time: " + minutes + ":" + seconds;
    }
}
