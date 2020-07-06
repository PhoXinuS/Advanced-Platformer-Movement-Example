using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    
    [SerializeField] Checker groundCheck = new Checker();
    [SerializeField] Checker ceilingCheck = new Checker();
    [SerializeField] Checker leftWallCheck = new Checker();
    [SerializeField] Checker rightWallCheck = new Checker();
    [SerializeField] string climbableTag = "Climbable";

    [SerializeField] CalculateHorizontalVelocity horizontalCalculator = new CalculateHorizontalVelocity();
    [SerializeField] CalculateVerticalVelocity verticalCalculator = new CalculateVerticalVelocity();

    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(gameObject); 
        
        groundCheck.SetUp(Vector2.down);
        ceilingCheck.SetUp(Vector2.up);
        leftWallCheck.SetUp(Vector2.left);
        rightWallCheck.SetUp(Vector2.right);
        
        horizontalCalculator.Setup(movementData, rigidBody2D, movementInput); 
        verticalCalculator.Setup(movementData, rigidBody2D, movementInput);
    }

    private void FixedUpdate()
    {
        bool isGrounded = groundCheck.IsInContactWithTarget();
        bool canStand = !ceilingCheck.IsInContactWithTarget();
        bool isTouchingLeftWall = leftWallCheck.IsInContactWithTarget();
        bool isTouchingRightWall = rightWallCheck.IsInContactWithTarget();
        bool isTouchingClimbableWall = leftWallCheck.IsInContactWithTarget(climbableTag) || rightWallCheck.IsInContactWithTarget(climbableTag);

        bool jumped = false;
        verticalCalculator.ApplyVelocity(isGrounded, canStand, isTouchingLeftWall, isTouchingRightWall, isTouchingClimbableWall, ref jumped);
        horizontalCalculator.ApplyVelocity(isGrounded, canStand, jumped, isTouchingLeftWall, isTouchingRightWall);
    }
}