using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpForce = 5f;    // Amount of force added when the player jumps
    private bool jumping = false;

    private Rigidbody2D rigidbody;
    private Vector2 moveDirection;
    public InputActions inputActions;

    [SerializeField] private LayerMask whatIsGround;    // A mask determining what is ground to the player
    [SerializeField] private Transform groundCheck; // A position marking where to check if the player is grounded
    [SerializeField] private Transform ceilingCheck;    // A position marking where to check for ceilings

    const float groundedRadius = .2f;   // Radius of the overlap circle to determine if grounded
	private bool grounded;  // Whether or not the player is grounded
	const float ceilingRadius = .2f;    // Radius of the overlap circle to determine if the player has hit a ceiling
    private bool facingRight = true;    // For determining which way the player is currently facing

    public UnityEvent onLandEvent;

	void Awake()
    {
        inputActions = new InputActions();
        inputActions.Player.Movement.performed += context => moveDirection = context.ReadValue<Vector2>();
        inputActions.Player.Movement.canceled += context => moveDirection = Vector2.zero;
        inputActions.Player.Jump.performed += context => jumping = true;
        inputActions.Player.Jump.canceled += context => jumping = false;

		rigidbody = GetComponent<Rigidbody2D>();

		if (onLandEvent == null)
        {
            onLandEvent = new UnityEvent();
        }
	}

	void OnEnable()
	{
		inputActions.Player.Enable();
	}

	void OnDisable()
	{
		inputActions.Player.Disable();
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

		Move(moveDirection.x, jumping);
    }

    void Move(float move, bool jumping)
    {
        if (grounded)
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
        }
        // If the player is on the ground and jumps...
        if (grounded && jumping)
        {
            // ... add vertical force to the player
            grounded = false;
            Jump();
        }
    }

    void Jump()
    {
        rigidbody.AddForce(new Vector2(0f, jumpForce));
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
}
