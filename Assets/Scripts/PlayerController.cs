// PlayerController.cs
// Unity 2021+ | Input System package
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Refernces")]
	private CharacterController controller;

	[SerializeField] private Camera playerCamera;

	[Header("Movement Settings")]
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float mouseSensitivity = 2f;

	[SerializeField] private float gravity = 9.81f;

	[SerializeField] private float jumpHeight = 20f;

	private float verticalVelocity; 

	private float xRotation = 0f;

	[SerializeField] private float sneakSpeed = 3f;
	[SerializeField] private float normalHeight = 1f;
	[SerializeField] private float sneakHeight = .5f;

	private bool isSneaking = false;

	[Header("Inputs")]
	private float moveInput;
	private float sideInput;

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		
	}

	private void Update()
	{
		InputManagement();
		Movement();
	}

	private void Movement()
	{
		GroundMovement();
	}
	private void GroundMovement()
	{
		Vector3 move = new Vector3(sideInput, 0, moveInput);
		move = transform.TransformDirection(move);

		// Sprint
		if (Input.GetKey(KeyCode.LeftShift) && !isSneaking)
			move *= moveSpeed * 5;
		else if (isSneaking)
			move *= sneakSpeed;
		else
			move *= moveSpeed;

		// Sneak toggle
		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (!isSneaking)
			{
				isSneaking = true;
				controller.height = sneakHeight;
			}
		}
		else
		{
			if (isSneaking)
			{
				isSneaking = false;
				controller.height = normalHeight;
			}
		}

		move.y = VerticalForceCalculation();
		controller.Move(move * Time.deltaTime);

		MouseInput();
		MouseGoInWindow();
}
	
	private void InputManagement()
	{
		moveInput = Input.GetAxis("Vertical");
		sideInput = Input.GetAxis("Horizontal");
	}
	private void MouseInput()
	{
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

		// Rotate player around Y axis
		transform.Rotate(Vector3.up * mouseX);

		// Apply vertical look rotation with clamp
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
	}

	private float VerticalForceCalculation()
	{
		if (controller.isGrounded)
		{
			verticalVelocity = -1f;

			if (Input.GetButtonDown("Jump"))
			{
				verticalVelocity = Mathf.Sqrt(jumpHeight * gravity * 2);
			}
			return verticalVelocity;
		} 
		verticalVelocity -= gravity * Time.deltaTime;
		return verticalVelocity;
	}	

	private void MouseGoInWindow()
	{
		if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked; // locks cursor to game window
            Cursor.visible = false;                   // hides it
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;   // unlocks cursor
            Cursor.visible = true;                    // shows it again
        }
	}
} 