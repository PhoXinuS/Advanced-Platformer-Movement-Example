using UnityEngine;

public class Movement : MonoBehaviour
{
    public MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;

    private float xVelocity = 0f;

    public Transform groundCheck;
    public LayerMask whatIsGround;
    private bool grounded;
    private bool doubleJumped;
    private bool pushedJumpButton;

    private void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        movementInput = new PlayerMovement(gameObject);
    }

    private void FixedUpdate()
    {
        var horizontalVelocity = CalculateHorizontalVelocity();
        var verticalVelocity = CalculateVerticalVelocity();
        rigidBody2D.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }
    
    
    private float CalculateHorizontalVelocity()
    {
        var horizontalInput = movementInput.movementInputNormalized.x;

        float timeMultiplier = Time.fixedDeltaTime * 500f;
        float horizontalTargetVelocity = horizontalInput * movementData.horizontalSpeed * timeMultiplier;

        float horizontalVelocity = 0f;
        if (Mathf.Abs(horizontalTargetVelocity) > Mathf.Abs(rigidBody2D.velocity.x))
        {
            horizontalVelocity = Mathf.SmoothDamp(rigidBody2D.velocity.x, horizontalTargetVelocity, ref xVelocity,
                movementData.accelerationTime);
        }
        else if (Mathf.Abs(horizontalTargetVelocity) < Mathf.Abs(rigidBody2D.velocity.x))
        {
            horizontalVelocity = Mathf.SmoothDamp(rigidBody2D.velocity.x, horizontalTargetVelocity, ref xVelocity,
                movementData.decelerationTime);
        }
        else
        {
            horizontalVelocity = horizontalTargetVelocity;
        }
        
        return horizontalVelocity;
    }
    
    private float CalculateVerticalVelocity()
    {
        //TODO: remove ground detection from this class to another marked with interface. Leave only input check and maybe Grounded / DoubleJump

        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.05f, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                doubleJumped = false;
            }
        }

        if (movementInput.movementInputNormalized.y > 0)
        {
            if (grounded && !pushedJumpButton)
            {
                pushedJumpButton = true;
                return movementData.jumpHeight;
            }
            else if (!doubleJumped && !pushedJumpButton)
            {
                doubleJumped = true;
                pushedJumpButton = true;
                return movementData.jumpHeight;
            }
        }
        else
        {
            pushedJumpButton = false;
        }

        return rigidBody2D.velocity.y;
    }
}
