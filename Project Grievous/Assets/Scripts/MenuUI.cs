using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void StartRun()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.Start);
    }

    public void ExitGame()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.Quit);
    }
}
