using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : MonoBehaviour
{
    public Transform sparkle;

    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("They are so pretty");
        var emission = GetComponent<ParticleSystem>().emission; // Stores the module in a local variable
        emission.enabled = true;

    }
}
