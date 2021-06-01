using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Destructible
{
    [SerializeField] ParticleSystem particles;
    
    public override void Destroy()
    {
        Debug.Log("MATARAM-ME");

        
        //TODO PLAY ANIMATION? PARTICLES!

        particles.Play();

        StartCoroutine(disableDoor(0.1f));

       

        Destroy(gameObject, 2f);
    }

    IEnumerator disableDoor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
    }
}
