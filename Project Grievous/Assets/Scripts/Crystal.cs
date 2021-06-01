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

        particles.Play();

        GetComponent<CapsuleCollider>().enabled = false;

        StartCoroutine(disableCrystal(0.1f));

        Destroy(gameObject, 2f);
    }

    IEnumerator disableCrystal(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<MeshRenderer>().enabled = false;
    }
}
