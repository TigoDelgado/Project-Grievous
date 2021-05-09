using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController Controller;

    [Header("Movement Variables")]
    [SerializeField] private float speed;
    private float horizontalDirection;
    private float verticalDirection;

    [Header("Ground Collision Variables")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    [Header("Jump Variables")]
    [SerializeField] private float lowJumpFallMultiplier = 5f;
    [SerializeField] private int extraJumps = 1;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float hangTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    private int extraJumpsValue;
    private float hangTimeCounter;
    private float jumpBufferCounter;
    private bool canJump => (jumpBufferCounter > 0 && (extraJumpsValue > 0 || hangTimeCounter > 0));

    [Header("Gravity Variables")]
    [SerializeField] private float gravity = -9.8f;
    private Vector3 velocity;


    // Update is called once per frame
    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBuffer;
        else
            jumpBufferCounter -= Time.deltaTime;

        horizontalDirection = Input.GetAxisRaw("Horizontal");
        verticalDirection = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (Controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            hangTimeCounter = hangTime;
            extraJumpsValue = extraJumps;
        }
        else
        {
            hangTimeCounter -= Time.fixedDeltaTime;
        }

        if (canJump)
        {
            if (!Controller.isGrounded) extraJumpsValue--;
            hangTimeCounter = 0f;
            jumpBufferCounter = 0f;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 move = horizontalDirection * transform.right + verticalDirection * transform.forward;
        Controller.Move(move.normalized * speed * Time.fixedDeltaTime);

        if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Force stop jump
            velocity.y += gravity * Time.fixedDeltaTime * lowJumpFallMultiplier;
            Controller.Move(velocity * Time.fixedDeltaTime);
        }
        else
        {
            // Apply gravity
            velocity.y += gravity * Time.fixedDeltaTime;
            Controller.Move(velocity * Time.fixedDeltaTime);
        }      
    }
}
