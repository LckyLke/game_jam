// PlayerController.cs
// Unity 2021+ | works with Legacy Input; set Input Handling = Both
using UnityEngine;
using UnityEngine.InputSystem;
//maxis penis ist klein
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    [SerializeField] private Camera playerCamera;

    [Header("Movement Settings")]
    private float moveSpeed;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;

    [Header("Crouching")]
    public float crouchSpeed = 5f;
	public float crouchYScale = 0.5f;
	private float startYScale;

    public enum MovementState { walking, sprinting, air, crouching }
    private MovementState state;

    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float gravity = 9.81f;   
    [SerializeField] private float jumpHeight = 2.0f; 

    private float verticalVelocity;
    private float xRotation = 0f;

    [Header("Inputs")]
    private float moveInput;
    private float sideInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveSpeed = walkSpeed; 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		startYScale = transform.localScale.y; 
    }

    private void Update()
    {
        StateHandler();
        InputManagement();
        Movement();
    }

    private void Movement()
    {
        GroundMovement();
        MouseInput();
        MouseGoInWindow();
    }

    private void GroundMovement()
    {
        Vector3 moveXZ = (transform.right * sideInput + transform.forward * moveInput) * moveSpeed;

        verticalVelocity = VerticalForceCalculation();

        Vector3 move = new Vector3(moveXZ.x, verticalVelocity, moveXZ.z);
        controller.Move(move * Time.deltaTime);
    }

    private void StateHandler()
    {
		bool grounded = controller.isGrounded;

		if (Input.GetKey(KeyCode.LeftControl) && state != MovementState.air)
		{
			state = MovementState.crouching;
			moveSpeed = crouchSpeed;
		}

        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void InputManagement()
    {
        moveInput = Input.GetAxisRaw("Vertical");
		sideInput = Input.GetAxisRaw("Horizontal");

		if (Input.GetKeyDown(KeyCode.LeftControl) && state != MovementState.air)
		{
			transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
			controller.Move(Vector3.down * 100f);
		}
		
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
		}
    }

    private void MouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            float v = -1f;

            if (Input.GetButtonDown("Jump"))
            {
                v = Mathf.Sqrt(2f * gravity * Mathf.Max(0.01f, jumpHeight));
            }
            return v;
        }

        verticalVelocity -= gravity * Time.deltaTime;
        return verticalVelocity;
    }

    private void MouseGoInWindow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
