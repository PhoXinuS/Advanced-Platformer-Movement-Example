﻿using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    
    [SerializeField] GroundCheck groundCheck = new GroundCheck();
    [SerializeField] CeilingCheck ceilingCheck = new CeilingCheck();

    [SerializeField] CalculateHorizontalVelocity horizontalCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalCalculator = new CalculateVerticalVelocity();

    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    private bool isGrounded;
    private bool canStand;
    
    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(gameObject); 
        
        groundCheck.SetUp(new []{gameObject});
        ceilingCheck.SetUp(new []{gameObject});
        
        horizontalCalculator.Setup(movementData, rigidBody2D, movementInput); 
        verticalCalculator.Setup(movementData, rigidBody2D, movementInput);
    }

    private void FixedUpdate()
    {
        isGrounded = groundCheck.IsTouchingGround();
        canStand = ceilingCheck.CanStand();

        verticalCalculator.ApplyVelocity(isGrounded);
        horizontalCalculator.ApplyVelocity(isGrounded, canStand);
    }
}