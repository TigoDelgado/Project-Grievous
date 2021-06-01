using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Destructible
{
    [SerializeField] ParticleSystem particles;
    
    public override void Destroy()
    {
        Debug.Log("MATARAM-ME");

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        //TODO PLAY ANIMATION? PARTICLES!

        particles.Play();

        Destroy(gameObject, 2f);
    }
}
