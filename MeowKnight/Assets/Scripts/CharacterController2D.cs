using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float jumpForce = 400f;
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Transform ceilingCheck;

	const float groundedRadius = .2f;   // Radius of the overlap circle to determine if grounded
	private bool grounded;  // Whether or not the character is ground
	const float ceilingRadius = .2f;    // Radius of the overlap circle to determine if the character can stand up

	private Rigidbody2D rigidbody2D;
	private bool facingRight = true;    // For determining which way the character is currently facing
	private Vector3 velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent onLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();

		if (onLandEvent == null)
		{
			onLandEvent = new UnityEvent();
		}
	}

	private void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;

		// The character is grounded if a circlecast to the groundcheck position hits anything designated as ground
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
	}

	public void Move(float move, bool jump)
	{
		// Only control character if grounded is true
		if (grounded)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to character
			rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);

			// If the input is moving the character right and the character is facing left...
			if (move > 0 && !facingRight)
			{
				// ... flip the character
				Flip();
			}
			//	Otherwise if the input is moving the character left and the player is facing right...
			else if (move < 0 && facingRight)
			{
				// ... flip the player
				Flip();
			}
		}
		// If the character should jump...
		if (grounded && jump)
		{
			// ... add vertical force to the character
			grounded = false;
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));
		}
	}

	private void Flip()
	{
		// Switch the way the character is labelled as facing
		facingRight = !facingRight;

		// Multiply the character's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
