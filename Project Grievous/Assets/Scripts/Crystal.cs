using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Destructible
{
    [SerializeField] int score = 1;
    
    public override void Destroy()
    {
        Debug.Log("MATARAM-ME E EU SOU UM CRISTAL!");

        ScoreManager.Instance?.AddScore(score);
        //TODO PLAY ANIMATION? PARTICLES!

        Destroy(gameObject);
    }
}
