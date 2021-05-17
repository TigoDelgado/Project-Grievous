using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField]
    float projectileSpeed = 0.5f;

    Vector3 shootDirection;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this, 20f);
    }

    public void Setup(Vector3 shootDirection)
    {
        this.shootDirection = shootDirection.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(shootDirection);
        transform.rotation = targetRotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += shootDirection * projectileSpeed * Time.deltaTime;   
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<FirstPersonMovement>().Die();
        }
        Destroy(gameObject);
    }

}
