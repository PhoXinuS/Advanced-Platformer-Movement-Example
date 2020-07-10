using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    
    [Space] [Header("Checkers")] [Space]
    [SerializeField] string climbableTag = "Climbable";
    [Space]
    [SerializeField] Checker groundCheck = new Checker();
    [SerializeField] Checker ceilingCheck = new Checker();
    [SerializeField] Checker ceilingCrouchCheck = new Checker();
    [SerializeField] Checker leftWallCheck = new Checker();
    [SerializeField] Checker rightWallCheck = new Checker();
    
    [Space] [Header("Velocity Calculators")] [Space]
    [SerializeField] CalculateHorizontalVelocity horizontalCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalCalculator = new CalculateVerticalVelocity();
    [SerializeField] Flipper flipper = new Flipper();

    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private IMovementInput movementInput;
    
    [SerializeField] string animInAir = "isInAir";
    [SerializeField] string animHorizontalSpeed = "xSpeedAbsolute";
    [SerializeField] string animVerticalSpeed = "ySpeed";

    private int animInAirHashed;
    private int animHorizontalSpeedHashed;
    private int animVerticalSpeedHashed;

    private void Start()
    {
        GameObject characterGameObject = gameObject;
        rigidBody2D = characterGameObject.GetComponent<Rigidbody2D>();
        animator = characterGameObject.GetComponent<Animator>();
        movementInput = new PlayerMovement(characterGameObject); 
        
        groundCheck.Setup(Vector2.down);
        ceilingCrouchCheck.Setup(Vector2.up);
        ceilingCheck.Setup(Vector2.up);
        leftWallCheck.Setup(Vector2.left);
        rightWallCheck.Setup(Vector2.right);
        
        horizontalCalculator.Setup(movementData, rigidBody2D, animator, movementInput); 
        verticalCalculator.Setup(movementData, rigidBody2D, animator, movementInput);
        flipper.Setup(rigidBody2D, transform);
        
        animInAirHashed = Animator.StringToHash(animInAir);
        animHorizontalSpeedHashed = Animator.StringToHash(animHorizontalSpeed);
        animVerticalSpeedHashed = Animator.StringToHash(animVerticalSpeed);
    }

    private void FixedUpdate()
    {
        bool isGrounded = groundCheck.IsInContactWithTarget();
        bool canStand = !ceilingCrouchCheck.IsInContactWithTarget();
        bool isTouchingLeftWall = leftWallCheck.IsInContactWithTarget();
        bool isTouchingRightWall = rightWallCheck.IsInContactWithTarget();
        bool isTouchingClimbableWall = leftWallCheck.IsInContactWithTarget(climbableTag) || rightWallCheck.IsInContactWithTarget(climbableTag);
        bool isTouchingClimbableCeiling = ceilingCheck.IsInContactWithTarget(climbableTag);
        
        bool jumped = false;
        verticalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingClimbableCeiling, isTouchingLeftWall, isTouchingRightWall, isTouchingClimbableWall,  ref jumped);
        horizontalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingClimbableCeiling, jumped, isTouchingLeftWall, isTouchingRightWall);
        flipper.ApplyFlip();
        
        animator.SetFloat(animHorizontalSpeedHashed, Mathf.Abs(rigidBody2D.velocity.x));
        animator.SetFloat(animVerticalSpeedHashed, rigidBody2D.velocity.y);
        if (!isTouchingLeftWall  && !isTouchingRightWall && canStand && !isGrounded)
        {
            animator.SetBool(animInAirHashed, true);
        }
        else
        {
            animator.SetBool(animInAirHashed, false);
        }
    }
}