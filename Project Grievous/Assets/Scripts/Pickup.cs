using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 25 * Time.deltaTime);
    }

    public void PickUp() // FIXME fazer coisas e outras coisas também!
    {
        Debug.Log("APANHARAM-ME!");
        Destroy(gameObject);
    }
}
