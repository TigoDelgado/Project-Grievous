using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    [SerializeField] float lookSensitivity = 1f;
    [Tooltip("Used to flip the vertical input axis")]
    [SerializeField] bool invertYAxis = false;
    [Tooltip("Used to flip the horizontal input axis")]
    [SerializeField] bool invertXAxis = false;

    bool m_FireInputWasHeld;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")){
            GameManager.Instance.TogglePause();
        }
    }


    public bool CanProcessInput()
    {
        return Cursor.lockState == CursorLockMode.Locked;
    }

    public Vector3 GetMoveInput()
    {
        if (CanProcessInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
            move = Vector3.ClampMagnitude(move, 1);

            return move;
        }

        return Vector3.zero;
    }

    public float GetLookInputsHorizontal()
    {
        if (CanProcessInput())
        {
            float i = Input.GetAxisRaw("Mouse X");

            // handle inverting vertical input
            if (invertXAxis)
                i *= -1f;

            // apply sensitivity multiplier
            i *= lookSensitivity;

            // reduce mouse input amount to be equivalent to stick movement
            i *= 0.01f;

            return i;
        }

        return 0f;
    }

    public float GetLookInputsVertical()
    {
        if (CanProcessInput())
        {
            float i = Input.GetAxisRaw("Mouse Y");

            // handle inverting vertical input
            if (invertYAxis)
                i *= -1f;

            // apply sensitivity multiplier
            i *= lookSensitivity;

            // reduce mouse input amount to be equivalent to stick movement
            i *= 0.01f;

            return i;
        }
        return 0f;
    }

    public bool GetJumpInputDown()
    {
        if (CanProcessInput())
        {
            return Input.GetButtonDown("Jump");
        }

        return false;
    }

    public bool GetJumpInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton("Jump");
        }

        return false;
    }

    public bool GetSprintInputHeld()
    {
        if (CanProcessInput())
        {
            return Input.GetButton("Sprint");
        }

        return false;
    }
}
