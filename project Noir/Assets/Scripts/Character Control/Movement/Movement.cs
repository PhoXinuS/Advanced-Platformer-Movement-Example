using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    
    [Space] [Header("Checkers")] [Space]
    [SerializeField] string climbableTag = "Climbable";
    [Space]
    [SerializeField] Raycast2DChecker groundCheck = new Raycast2DChecker();
    [SerializeField] Raycast2DChecker ceilingCheck = new Raycast2DChecker();
    [SerializeField] Raycast2DChecker ceilingCrouchCheck = new Raycast2DChecker();
    [SerializeField] Raycast2DChecker leftWallCheck = new Raycast2DChecker();
    [SerializeField] Raycast2DChecker rightWallCheck = new Raycast2DChecker();
    [SerializeField] Raycast2DChecker leftWallClimbCheck = new Raycast2DChecker();
    [SerializeField] Raycast2DChecker rightWallClimbCheck = new Raycast2DChecker();

    [Space] [Header("Velocity Calculators")] [Space]
    [SerializeField] CalculateHorizontalVelocity horizontalCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalCalculator = new CalculateVerticalVelocity();
    [SerializeField] CalculateLedge ledgeCalculator = new CalculateLedge();
    [SerializeField] CalculateLedgeSlip ledgeSlipCalculator = new CalculateLedgeSlip();
    [SerializeField] Flipper flipper = new Flipper();

    private Rigidbody2D rb2D;
    private Animator animator;
    private IMovementInput movementInput;
    
    [Space] [Header("Animator Variables")] [Space]
    [SerializeField] string animGrounded = "isGrounded";
    [SerializeField] string animInAir = "isInAir";
    [SerializeField] string animHorizontalSpeed = "xSpeedAbsolute";
    [SerializeField] string animVerticalSpeed = "ySpeed";

    private int animGroundedHashed;
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
        rb2D = characterGameObject.GetComponent<Rigidbody2D>();
        animator = characterGameObject.GetComponent<Animator>();
        movementInput = new PlayerMovementInput(characterGameObject); 
        
        groundCheck.Setup(Vector2.down);
        ceilingCrouchCheck.Setup(Vector2.up);
        ceilingCheck.Setup(Vector2.up);
        leftWallCheck.Setup(Vector2.left);
        rightWallCheck.Setup(Vector2.right);     
        leftWallClimbCheck.Setup(Vector2.left);
        rightWallClimbCheck.Setup(Vector2.right);
        
        horizontalCalculator.Setup(movementData, rb2D, animator, movementInput); 
        verticalCalculator.Setup(movementData, rb2D, animator, movementInput);
        ledgeCalculator.Setup(rb2D, animator, movementInput);
        ledgeSlipCalculator.Setup(rb2D, animator, movementInput);
        flipper.Setup(rb2D, transform);
        
        animGroundedHashed = Animator.StringToHash(animGrounded);
        animInAirHashed = Animator.StringToHash(animInAir);
        animHorizontalSpeedHashed = Animator.StringToHash(animHorizontalSpeed);
        animVerticalSpeedHashed = Animator.StringToHash(animVerticalSpeed);
    }

    private void FixedUpdate()
    {
        isTouchingLeftWall = false;
        isTouchingRightWall = false;
        if (flipper.flipped)
        {
            leftWallClimbCheck.Setup(Vector2.right);
            rightWallClimbCheck.Setup(Vector2.left);
            leftWallCheck.Setup(Vector2.right);
            rightWallCheck.Setup(Vector2.left);       
            isTouchingLeftWall = rightWallCheck.IsInContactWithTarget();
            isTouchingRightWall = leftWallCheck.IsInContactWithTarget();
        }
        else
        {
            leftWallClimbCheck.Setup(Vector2.left);
            rightWallClimbCheck.Setup(Vector2.right);
            leftWallCheck.Setup(Vector2.left);
            rightWallCheck.Setup(Vector2.right);
            isTouchingLeftWall = leftWallCheck.IsInContactWithTarget();
            isTouchingRightWall = rightWallCheck.IsInContactWithTarget();
        }
        isGrounded = groundCheck.IsInContactWithTarget();
        canStand = !ceilingCrouchCheck.IsInContactWithTarget();
        isTouchingClimbableWall = leftWallClimbCheck.IsInContactWithTarget(climbableTag) || rightWallClimbCheck.IsInContactWithTarget(climbableTag);
        isTouchingClimbableCeiling = ceilingCheck.IsInContactWithTarget(climbableTag);
        
        verticalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingClimbableCeiling, isTouchingLeftWall, isTouchingRightWall, isTouchingClimbableWall);
        horizontalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingClimbableCeiling, verticalCalculator.jumped, isTouchingLeftWall, isTouchingRightWall);
        ledgeCalculator.ApplyLedge(flipper.flipped);
        ledgeSlipCalculator.ApplyLedgeSlip(flipper.flipped, verticalCalculator.isClimbingWalls, horizontalCalculator.IsCrouching());
        
        bool isOnLedge = ledgeCalculator.ledgeClimb.isClimbingLedge || ledgeCalculator.ledgeDangle.isDangling ||
                         ledgeSlipCalculator.ledgeSlip.isSlippingLedge;
        flipper.ApplyFlip(isGrounded, isOnLedge, isTouchingLeftWall, isTouchingRightWall);
        
        SetAnimator();
    }

    private void SetAnimator()
    {
        animator.SetFloat(animHorizontalSpeedHashed, Mathf.Abs(rb2D.velocity.x));
        animator.SetFloat(animVerticalSpeedHashed, rb2D.velocity.y);
        if (isGrounded)
        {
            animator.SetBool(animGroundedHashed, true);
        }
        else
        {
            animator.SetBool(animGroundedHashed, false);
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


    #region Called from AnimationEvents
    
    public void ClimbedLedge()
    {
        ledgeCalculator.ledgeClimb.Climbed();
        ledgeCalculator.ledgeDangle.CancelDangle();
    }
    public void SlippedLedge()
    {
        ledgeSlipCalculator.ledgeSlip.Slipped();
    }
    #endregion
    
}