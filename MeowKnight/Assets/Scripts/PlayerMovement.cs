using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public CharacterController2D controller;
    public InputActions inputActions;

    private Vector2 moveInput;
    [SerializeField] public float moveSpeed = 5f;
    private bool jump = false;

    void Awake()
    {
        inputActions = new InputActions();
		inputActions.Enable();
		inputActions.Player.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        //inputActions.Player.Movement.canceled += context => moveInput = Vector2.zero;

        inputActions.Player.Jump.performed += context => jump = true;
    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(moveInput.x * Time.fixedDeltaTime, jump);
        jump = false;
    }

    void Move(Vector2 direction)
    {
        moveInput.x = direction.x * moveSpeed;
    }
}
