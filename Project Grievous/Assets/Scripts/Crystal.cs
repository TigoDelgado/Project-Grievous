using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Destructible
{
    [SerializeField] int score = 1;

    [SerializeField] ParticleSystem particles;
    
    public override void Destroy()
    {
        Debug.Log("MATARAM-ME E EU SOU UM CRISTAL!");

        ScoreManager.Instance?.AddScore(score);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        //TODO PLAY ANIMATION? PARTICLES!

        particles.Play();

        Destroy(gameObject, 2f);
    }
}
