using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;


public class WallRun : MonoBehaviour
{
    [Header("Detection variables")]
    [Tooltip("Max distance to detect wall")]
    [SerializeField] float wallMaxDistance = 1;
    [Tooltip("Minimum distance from floor required to wallrun")]
    [SerializeField] float minimumHeight = 1.2f;
    [Tooltip("Duration to wait after jump before attaching to wall")]
    [SerializeField] float jumpDuration = 1;
    [Tooltip("Wall Layers")]
    [SerializeField] LayerMask wallLayer = -1;

    [Header("Wall running modifiers")]
    [Tooltip("Speed multiplier while wallrunning")]
    [SerializeField] float wallSpeedMultiplier = 1.2f;
    [Tooltip("Strenght of wall jump multiplier")]
    [SerializeField] float wallBouncing = 3;
    [Tooltip("Gravity force while wallrunning")]
    [SerializeField] float wallGravityDownForce = 20f;

    [Header("Camera")]
    [Tooltip("Camera rotation while wallrunning")]
    [SerializeField] float maxAngleRoll = 20;
    [Tooltip("How fast camera rotates")]
    [SerializeField] float cameraTransitionDuration = 1;
    [Range(0.0f, 1.0f)]
    [SerializeField] float normalizedAngleThreshold = 0.2f;

    [Header("Extra")]
    [Tooltip("Press sprint to wallrun")]
    [SerializeField] bool useSprint;

    FirstPersonMovement m_PlayerCharacterController;
    PlayerInputHandler m_InputHandler;

    Vector3[] directions;
    RaycastHit[] hits;

    bool isWallRunning = false;
    Vector3 lastWallPosition;
    Vector3 lastWallNormal;
    float elapsedTimeSinceJump = 0;
    float elapsedTimeSinceWallAttach = 0;
    float elapsedTimeSinceWallDetatch = 0;
    bool jumping;
    //float lastVolumeValue = 0;
    //float noiseAmplitude;

    bool isPlayergrounded() => m_PlayerCharacterController.isGrounded;

    public bool IsWallRunning() => isWallRunning;

    bool CanWallRun()
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        bool isSprinting = m_InputHandler.GetSprintInputHeld();
        isSprinting = !useSprint ? true : isSprinting;

        return !isPlayergrounded() && verticalAxis > 0 && VerticalCheck() && isSprinting;
    }

    bool VerticalCheck()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }


    void Start()
    {
        m_PlayerCharacterController = GetComponent<FirstPersonMovement>();
        m_InputHandler = GetComponent<PlayerInputHandler>();

        directions = new Vector3[]{
            Vector3.right,
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };
    }


    public void LateUpdate()
    {
        isWallRunning = false;

        if (m_InputHandler.GetJumpInputDown())
        {
            jumping = true;
        }

        if (CanAttach())
        {
            hits = new RaycastHit[directions.Length];

            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                Physics.Raycast(transform.position, dir, out hits[i], wallMaxDistance, wallLayer);
                if (hits[i].collider != null)
                {
                    Debug.DrawRay(transform.position, dir * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, dir * wallMaxDistance, Color.red);
                }
            }

            if (CanWallRun())
            {
                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if (hits.Length > 0)
                {
                    OnWall(hits[0]);
                    lastWallPosition = hits[0].point;
                    lastWallNormal = hits[0].normal;
                }
            }
        }

        if (isWallRunning)
        {
            elapsedTimeSinceWallDetatch = 0;

            elapsedTimeSinceWallAttach += Time.deltaTime;
            m_PlayerCharacterController.characterVelocity += Vector3.down * wallGravityDownForce * Time.deltaTime;
        }
        else
        {
            elapsedTimeSinceWallAttach = 0;

            elapsedTimeSinceWallDetatch += Time.deltaTime;
        }
    }

    bool CanAttach()
    {
        if (jumping)
        {
            elapsedTimeSinceJump += Time.deltaTime;
            if (elapsedTimeSinceJump > jumpDuration)
            {
                elapsedTimeSinceJump = 0;
                jumping = false;
            }
            return false;
        }
        return true;
    }

    void OnWall(RaycastHit hit)
    {
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if (d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            Debug.DrawRay(transform.position, lastWallNormal * 10, Color.magenta);

            m_PlayerCharacterController.characterVelocity = alongWall * vertical * wallSpeedMultiplier;
            isWallRunning = true;
        }
    }

    float CalculateSide()
    {
        if (isWallRunning)
        {
            Vector3 heading = lastWallPosition - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
            float dir = Vector3.Dot(perp, transform.up);
            return dir;
        }
        return 0;
    }

    public float GetCameraRoll()
    {
        float dir = CalculateSide();
        float cameraAngle = m_PlayerCharacterController.playerCamera.transform.eulerAngles.z;
        float targetAngle = 0;
        if (dir != 0)
        {
            targetAngle = Mathf.Sign(dir) * maxAngleRoll;
        }
        return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(elapsedTimeSinceWallAttach, elapsedTimeSinceWallDetatch) / cameraTransitionDuration);
    }

    public Vector3 GetWallJumpDirection()
    {
        if (isWallRunning)
        {
            return lastWallNormal * wallBouncing + Vector3.up;
        }
        return Vector3.zero;
    }
}

