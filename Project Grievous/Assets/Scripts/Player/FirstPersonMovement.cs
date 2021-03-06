using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(Health))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Components")]
    public Camera playerCamera;

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    [SerializeField] float gravityDownForce = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    [SerializeField] LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    [SerializeField] float groundCheckDistance = 0.05f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    [SerializeField] float maxSpeedOnGround = 10f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    [SerializeField] float movementSharpnessOnGround = 15;
    [Tooltip("Max movement speed when not grounded")]
    [SerializeField] float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    [SerializeField] float accelerationSpeedInAir = 25f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    [SerializeField] float sprintSpeedModifier = 2f;
    [Tooltip("Height at which the player dies instantly when falling off the map")]
    [SerializeField] float killHeight = -6f;

    [Header("Interaction")]
    [SerializeField] float pickupRange = 5f;
    [SerializeField] float pickupRadius = 2f;
    [SerializeField] LayerMask interactablesMask = -1;

    [Header("Fall Damage")]
    [Tooltip("Whether the player will recieve damage when hitting the ground at high speed")]
    public bool recievesFallDamage;
    [Tooltip("Minimun fall speed for recieving fall damage")]
    public float minSpeedForFallDamage = 10f;
    [Tooltip("Fall speed for recieving th emaximum amount of fall damage")]
    public float maxSpeedForFallDamage = 30f;
    [Tooltip("Damage recieved when falling at the mimimum speed")]
    public float fallDamageAtMinSpeed = 10f;
    [Tooltip("Damage recieved when falling at the maximum speed")]
    public float fallDamageAtMaxSpeed = 50f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    [SerializeField] float rotationSpeed = 200f;

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    [SerializeField] float jumpForce = 9f;
    [SerializeField] private float hangTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;
    private float hangTimeCounter;
    private float jumpBufferCounter;

    [Header("Abilties")]
    [Tooltip("Shield prefab")]
    public Object pfShield;


    public Vector3 characterVelocity { get; set; }
    public bool isGrounded { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public bool isDead { get; private set; }
    public bool isCrouching { get; private set; }
    public float RotationMultiplier
    {
        get
        {
            return 1f;
        }
    }

    public int shields = 0;
    public int dashes = 0;

    private bool finished = false;

    PlayerInputHandler m_InputHandler;
    CharacterController m_Controller;
    WallRun wallRunComponent;
    Health m_health;
    Checkpoints m_checkpoints;

    Vector3 m_GroundNormal;
    Vector3 m_CharacterVelocity;
    Vector3 m_LatestImpactSpeed;

    Vector3 platformPosition;
    bool onPlatform = false;

    float m_LastTimeJumped = 0f;
    float m_CameraVerticalAngle = 0f;
    float m_footstepDistanceCounter;
    float m_TargetCharacterHeight;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;


    private void Start()
    {
        // fetch components on the same gameObject
        m_Controller = GetComponent<CharacterController>();

        m_InputHandler = GetComponent<PlayerInputHandler>();

        wallRunComponent = GetComponent<WallRun>();

        m_health = GetComponent<Health>();

        m_Controller.enableOverlapRecovery = true;

        m_checkpoints = GetComponent<Checkpoints>();
    }


    void Update()
    {
        // check for Y kill
        if (!isDead && transform.position.y < killHeight)
        {
            Die();
        }

        hasJumpedThisFrame = false;

        bool wasGrounded = isGrounded;

        AbilitiesCheck();
        GroundCheck();
        GroundTagCheck();

        // landing
        if (isGrounded && !wasGrounded)
        {
            // Fall damage
            float fallSpeed = -Mathf.Min(characterVelocity.y, m_LatestImpactSpeed.y);
            float fallSpeedRatio = (fallSpeed - minSpeedForFallDamage) / (maxSpeedForFallDamage - minSpeedForFallDamage);
            if (recievesFallDamage && fallSpeedRatio > 0f)
            {
                int dmgFromFall = (int) Mathf.Lerp(fallDamageAtMinSpeed, fallDamageAtMaxSpeed, fallSpeedRatio);
                m_health.TakeDamage(dmgFromFall, null);

                // fall damage SFX
                //audioSource.PlayOneShot(fallDamageSFX);
            }
            else
            {
                // land SFX
                //audioSource.PlayOneShot(landSFX);
            }
        }

        UpdateJump();

        HandleCharacterMovement();

        HandleCheckpointSet();
    }


    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
        {
            //if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            //if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height), m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            if (Physics.SphereCast(GetBottomSphereCenter(), m_Controller.radius - Physics.defaultContactOffset, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > m_Controller.skinWidth)
                    {
                        m_Controller.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
        
    }

    void GroundTagCheck()
    {
        if (Physics.SphereCast(GetBottomSphereCenter(), m_Controller.radius + Physics.defaultContactOffset, Vector3.down, out RaycastHit hit, groundCheckDistance))
        {
            // CHECK IF FINISHED LEVEL
            if (!finished && hit.transform.CompareTag("End"))
            {
                if (GameManager.Instance.state == GameManager.GameState.Running)
                {
                    finished = true;
                    GameManager.Instance.UpdateGameState(GameManager.GameState.End);
                }
            }


            // CHECK IF ON MOVING PLATFORM
            if (hit.transform.CompareTag("MovingPlatform"))
            {
                if (onPlatform)
                {
                    m_Controller.Move(hit.transform.position - platformPosition);
                }
                platformPosition = hit.transform.position;
                onPlatform = true;
            }

            else
            {
                onPlatform = false;
            }
        }
        else
        {
            onPlatform = false;
        }
    }

    void UpdateJump()
    {
        if (isGrounded)
        {
            hangTimeCounter = hangTime;
        }
        else
        {
            hangTimeCounter -= Time.deltaTime;
        }

        if (m_InputHandler.GetJumpInputDown())
        {
            jumpBufferCounter = jumpBuffer;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void HandleCharacterMovement()
    {
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * rotationSpeed * RotationMultiplier), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * rotationSpeed * RotationMultiplier;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            if (wallRunComponent != null)
            {
                playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, wallRunComponent.GetCameraRoll());
            }
            else
            {
                playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
            }
        }

        // character movement handling
        bool isSprinting = m_InputHandler.GetSprintInputHeld();
        {

            float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            // handle grounded movement
            if (isGrounded || (wallRunComponent != null && wallRunComponent.IsWallRunning()))
            {
                if (isGrounded)
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;

                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) * targetVelocity.magnitude;

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);
                }

                // jumping
                if ((hangTimeCounter > 0 || (wallRunComponent != null && wallRunComponent.IsWallRunning())) && jumpBufferCounter > 0)
                {
                    if (hangTimeCounter > 0)
                    {
                        // start by canceling out the vertical component of our velocity
                        characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                        // then, add the jumpSpeed value upwards
                        characterVelocity += Vector3.up * jumpForce;
                    }
                    else
                    {
                        characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                        // then, add the jumpSpeed value upwards
                        characterVelocity += wallRunComponent.GetWallJumpDirection() * jumpForce;
                    }

                    // remember last time we jumped because we need to prevent snapping to ground for a short time
                    m_LastTimeJumped = Time.time;
                    hasJumpedThisFrame = true;

                    // Force grounding to false
                    isGrounded = false;
                    m_GroundNormal = Vector3.up;
                }
            }

            // handle air movement
            else
            {
                if (wallRunComponent == null || (wallRunComponent != null && !wallRunComponent.IsWallRunning()))
                {
                    // calculate the desired velocity from inputs, max speed, and current slope
                    Vector3 targetHorizontalVelocity = Vector3.ProjectOnPlane( worldspaceMoveInput * maxSpeedOnGround * speedModifier, Vector3.up);

                    // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
                    Vector3 horizontalVelocity = Vector3.Lerp(Vector3.ProjectOnPlane(characterVelocity, Vector3.up), targetHorizontalVelocity, movementSharpnessOnGround / 4 * Time.deltaTime);

                    float verticalVelocity = characterVelocity.y;
                    characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                    /*// add air acceleration
                    characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

                    // limit air speed to a maximum, but only horizontally
                    float verticalVelocity = characterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
                    characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);*/

                    // apply the gravity to the velocity
                    characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
                }
            }
        }

        // apply the final calculated velocity value as a character movement
        Vector3 capsuleBottomBeforeMove = GetBottomSphereCenter();
        Vector3 capsuleTopBeforeMove = GetTopSphereCenter();
        m_Controller.Move(characterVelocity * Time.deltaTime);

        // detect obstructions to adjust velocity accordingly
        m_LatestImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
        {
            // We remember the last impact speed because the fall damage logic might need it
            m_LatestImpactSpeed = characterVelocity;

            characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
        }
    }

    Vector3 GetBottomSphereCenter()
    {
        return transform.position - (transform.up * (m_Controller.radius + Physics.defaultContactOffset));
    }

    Vector3 GetTopSphereCenter()
    {
        //return transform.position + (transform.up * (m_Controller.height - m_Controller.radius + Physics.defaultContactOffset));
        return transform.position + (transform.up * (m_Controller.height - m_Controller.radius));
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    }

    // Gets a reoriented direction that is tangent to a given slope
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    public void AbilitiesCheck()
    {
        if (m_InputHandler.GetQAbiliyInputDown() && shields > 0)
        {
            Object shieldTransform = Instantiate(pfShield, transform.position, Quaternion.identity);
            Destroy(shieldTransform, 2f);
            shields--;
        }

        if (m_InputHandler.GetEAbiliyInputDown() && dashes > 0)
        {
            float x = characterVelocity.x;
            float z = characterVelocity.z;
            characterVelocity = new Vector3(x, 0, z);
            characterVelocity += transform.forward * 15;
            characterVelocity += transform.up * 10;
            dashes--;
        }

        /*if (m_InputHandler.GetInteractInputDown())
        {
            Debug.Log("trying to interact");
            //Interact();
        }*/
    }

    private void Interact()
    {
        Collider[] hitColliders = Physics.OverlapSphere(GetInteractionSphereCenter(), pickupRadius - Physics.defaultContactOffset, interactablesMask, QueryTriggerInteraction.Ignore);
        foreach (Collider hitCollider in hitColliders)
        {
            Debug.Log("Collider Hit!!");
            if (hitCollider.GetComponent<Pickup>())
            {
                hitCollider.GetComponent<Pickup>().PickUp();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Pickup>())
        {
            other.GetComponent<Pickup>().PickUp();
            if (other.CompareTag("Shield")){
                shields++;
            }
            if (other.CompareTag("Dash"))
            {
                dashes++;
            }
        }
    }

    private Vector3 GetInteractionSphereCenter()
    {
        return transform.position + (transform.forward * (pickupRange - pickupRadius));
    }

    private void HandleCheckpointSet()
    {
        int checkPointInput = m_InputHandler.GetCheckpointInput();
        if (checkPointInput > 0)
        {
            m_checkpoints.SetOnCheckpoint(this.transform, checkPointInput);
            characterVelocity = new Vector3(0, 0, 0);
        }
    }

    // Player Death
    public void Die()
    {
        isDead = true;
        GameManager.Instance.UpdateGameState(GameManager.GameState.GameOver);
    }

    private void OnDrawGizmos()
    {
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        //Gizmos.DrawWireSphere(GetInteractionSphereCenter(), pickupRadius - Physics.defaultContactOffset);
    }
}
