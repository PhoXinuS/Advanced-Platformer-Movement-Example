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
    
    [SerializeField] string animGrounded = "isGrounded";
    [SerializeField] string animInAir = "isInAir";
    [SerializeField] string animHorizontalSpeed = "xSpeedAbsolute";
    [SerializeField] string animVerticalSpeed = "ySpeed";

    private int animInGroundedHashed;
    private int animInAirHashed;
    private int animHorizontalSpeedHashed;
    private int animVerticalSpeedHashed;

    private bool isGrounded;
    private bool canStand;
    private bool isTouchingClimbableWall;
    private bool isTouchingClimbableCeiling;
    private bool isTouchingLeftWall;
    private bool isTouchingRightWall;

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
        
        animInGroundedHashed = Animator.StringToHash(animGrounded);
        animInAirHashed = Animator.StringToHash(animInAir);
        animHorizontalSpeedHashed = Animator.StringToHash(animHorizontalSpeed);
        animVerticalSpeedHashed = Animator.StringToHash(animVerticalSpeed);
    }

    private void FixedUpdate()
    {
        
        isGrounded = groundCheck.IsInContactWithTarget();
        canStand = !ceilingCrouchCheck.IsInContactWithTarget();
        isTouchingClimbableWall = leftWallCheck.IsInContactWithTarget(climbableTag) || rightWallCheck.IsInContactWithTarget(climbableTag);
        isTouchingClimbableCeiling = ceilingCheck.IsInContactWithTarget(climbableTag);
        isTouchingLeftWall = false;
        isTouchingRightWall = false;
        if (flipper.flipped)
        {
            leftWallCheck.Setup(Vector2.right);
            rightWallCheck.Setup(Vector2.left);
            isTouchingLeftWall = rightWallCheck.IsInContactWithTarget();
            isTouchingRightWall = leftWallCheck.IsInContactWithTarget();
        }
        else
        {
            leftWallCheck.Setup(Vector2.left);
            rightWallCheck.Setup(Vector2.right);
            isTouchingLeftWall = leftWallCheck.IsInContactWithTarget();
            isTouchingRightWall = rightWallCheck.IsInContactWithTarget();
        }

        bool jumped = false;
        verticalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingClimbableCeiling, isTouchingLeftWall, isTouchingRightWall, isTouchingClimbableWall,  ref jumped);
        horizontalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingClimbableCeiling, jumped, isTouchingLeftWall, isTouchingRightWall);
        flipper.ApplyFlip();
        
        SetAnimator();
    }

    private void SetAnimator()
    {
        animator.SetFloat(animHorizontalSpeedHashed, Mathf.Abs(rigidBody2D.velocity.x));
        animator.SetFloat(animVerticalSpeedHashed, rigidBody2D.velocity.y);
        if (isGrounded)
        {
            animator.SetBool(animInGroundedHashed, true);
        }
        else
        {
            animator.SetBool(animInGroundedHashed, false);
        }

        if (!isTouchingLeftWall && !isTouchingRightWall && canStand && !isGrounded)
        {
            animator.SetBool(animInAirHashed, true);
        }
        else
        {
            animator.SetBool(animInAirHashed, false);
        }
    }
}