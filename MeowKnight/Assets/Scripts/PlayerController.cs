using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

	public float moveSpeed = 5f;

	public float minJumpForce = 1f;   // Min jump force
	public float maxJumpForce = 8f;   // Max jump force
	public float jumpForceIncrement = .1f;   // Force increment
	public float currentJumpForce;    // Current amount of force added when the player jumps
	private bool jumping = false;

	public float bounceForce = 100f;
	public LayerMask wallLayer;

	private Rigidbody2D rigidbody;
	private Vector2 moveDirection;
	public InputActions inputActions;

	[SerializeField] private LayerMask whatIsGround;    // A mask determining what is ground to the player
	[SerializeField] private Transform groundCheck; // A position marking where to check if the player is grounded
	[SerializeField] private Transform ceilingCheck;    // A position marking where to check for ceilings

	const float groundedRadius = .5f;   // Radius of the overlap circle to determine if grounded
	private bool grounded;  // Whether or not the player is grounded
	const float ceilingRadius = .5f;    // Radius of the overlap circle to determine if the player has hit a ceiling
	private bool facingRight = true;    // For determining which way the player is currently facing

	public UnityEvent onLandEvent;

	public Animator animator;

	public GameObject winMenu;
	public GameObject winMenuSelectedButton;

	public GameObject pauseMenu;
	public GameObject pauseMenuSelectedButton;

	void Awake()
	{
		inputActions = new InputActions();
		inputActions.Player.Movement.performed += context => moveDirection = context.ReadValue<Vector2>();
		inputActions.Player.Movement.canceled += context => moveDirection = Vector2.zero;

		rigidbody = GetComponent<Rigidbody2D>();

		animator = GetComponent<Animator>();

		if (onLandEvent == null)
		{
			onLandEvent = new UnityEvent();
		}
	}

	void OnEnable()
	{
		inputActions.Player.Enable();

		inputActions.Player.Pause.performed += OnPause;
	}

	void OnDisable()
	{
		inputActions.Player.Disable();

		inputActions.Player.Pause.performed -= OnPause;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;
				if (!wasGrounded)
				{
					onLandEvent.Invoke();
				}
			}
		}
		if (grounded && !jumping)
		{
			Move(moveDirection.x);
		}
	}

	void Update()
	{
		Debug.Log("jumping = " + jumping);
		Debug.Log("grounded = " + grounded);
		if (grounded)
		{
			animator.SetBool("falling", false);

			if (jumping)
			{
				currentJumpForce += jumpForceIncrement * Time.deltaTime;

				if (currentJumpForce >= maxJumpForce)
				{
					Jump();
				}
			}
			else
			{
				animator.SetBool("landed", true);
			}
		}
		else
		{
			animator.SetBool("landed", false);

			animator.SetBool("falling", true);
		}

		float move = inputActions.Player.Movement.ReadValue<Vector2>().x;

		if (jumping)
		{
			if (move > 0 && !facingRight)
			{
				Flip();
			}
			else if (move < 0 && facingRight)
			{
				Flip();
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			// Calculate the direction to bounce in
			Vector2 bounceDirection = collision.contacts[0].normal;
			// Apply the bounce force
			rigidbody.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
		}

		if (collision.gameObject.CompareTag("Goal"))
		{
			// Completed level
			Time.timeScale = 0f;
			winMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(winMenuSelectedButton);
		}
	}

	void Move(float move)
	{
		if (grounded && !jumping)
		{
			// Move the player using velocity
			rigidbody.velocity = new Vector2(moveDirection.x * moveSpeed, rigidbody.velocity.y);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
			{
				// ... flip the player
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && facingRight)
			{
				// ... flip the player
				Flip();
			}

			if (move != 0)
			{
				animator.SetBool("moving", true);
			}
			else
			{
				animator.SetBool("moving", false);
			}
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{

		Debug.Log("OnJump called");

		if (context.started)
		{
			currentJumpForce = minJumpForce;
			jumping = true;
			grounded = false;   // grounded is false so that player cannot move in air

			animator.SetBool("jumpStart", true);
		}
		else if (context.canceled)
		{
			animator.SetBool("jumpStart", false);
			Jump();
		}
	}

	private void Jump()
	{
		jumping = false;
		grounded = true;

		// If the player is on the ground and jumps...
		if (grounded)
		{
			// ... add vertical force to the player
			rigidbody.AddForce(new Vector2(0f, currentJumpForce), ForceMode2D.Impulse);
			currentJumpForce = minJumpForce;
		}
	}

	private void Flip()
	{
		// Switch the way the player is facing
		facingRight = !facingRight;

		// Multiply the player's lcoal scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnPause(InputAction.CallbackContext context)
	{
		// The Pause input action was triggered, so toggle the pause state and show or hide the pause menu
		if (pauseMenu.activeSelf)
		{
			pauseMenu.SetActive(false);
			Time.timeScale = 1f;
		}
		else
		{
			pauseMenu.SetActive(true);
			EventSystem.current.SetSelectedGameObject(pauseMenuSelectedButton);
			Time.timeScale = 0f;
		}
	}
}
