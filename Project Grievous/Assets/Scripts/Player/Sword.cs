using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    FirstPersonMovement m_CharacterController;
    PlayerInputHandler m_InputHandler;
    [SerializeField] Animator animator;
    [SerializeField] Transform sword;

    [SerializeField] float range = 5f;
    [SerializeField] float radius = 2f;
    [SerializeField] LayerMask enemyLayers = -1;

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = gameObject.GetComponent<FirstPersonMovement>();
        m_InputHandler = gameObject.GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator?.Play("sword_swing");
            Swing();
        }
        else if (!isPlaying("sword_swing") 
            && m_CharacterController.isGrounded 
            && m_InputHandler.GetMoveInput() != new Vector3(0, 0, 0))
        {
            animator?.Play("sword_running");
        }
        else if (!isPlaying("sword_swing") && m_CharacterController.isGrounded)
        {
            animator?.Play("sword_idle");
        }
        else if (!isPlaying("sword_swing"))
        {
            animator?.Play("sword_air");
            //m_animator.CrossFade("sword_air", 0.1f);
        }
    }

    bool isPlaying(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public void Swing()
    {
        Collider[] hitColliders = Physics.OverlapSphere(GetSphereCenter(), radius - Physics.defaultContactOffset, enemyLayers, QueryTriggerInteraction.Ignore);
        foreach (Collider hitCollider in hitColliders)
        {
            Debug.Log("collider hit");
            if (hitCollider.GetComponent<Destructible>())
            {
                hitCollider.GetComponent<Destructible>().Destroy();
            }
        }

    }

    private Vector3 GetSphereCenter()
    {
        return transform.position + (sword.forward * (range - radius));
    }

    private void OnDrawGizmos()
    {
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(GetSphereCenter(), radius);
    }

    public void Die()
    {
        m_CharacterController.Die();
    }
}