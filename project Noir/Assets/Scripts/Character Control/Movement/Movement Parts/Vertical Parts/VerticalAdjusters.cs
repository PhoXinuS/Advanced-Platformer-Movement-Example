using UnityEngine;

[System.Serializable]
public class VerticalAdjusters
{
    [SerializeField] float fallMultiplier = 2.5f;

    private bool wasPushingJumpButton;
    private bool isPushingJumpButton;
    private bool alreadyAdjustedJumpHeight;
    
    private Rigidbody2D rb2D;
    private IMovementInput movementInput;

    internal void SetUp(Rigidbody2D rb2D, IMovementInput movementInput)
    {

        this.rb2D = rb2D;
        this.movementInput = movementInput;
    }

    internal float ApplyAdjusters(float verticalVelocity, bool jumped, bool betterFall, bool adjustJump)
    {
        if (jumped)
        {
            alreadyAdjustedJumpHeight = false;
        }
        if (betterFall)
        {
            verticalVelocity += BetterFallingVelocity();
        }
        if (adjustJump)
        {
            verticalVelocity += AdjustJumpHeight();
        }
        return verticalVelocity;
    }

    private float BetterFallingVelocity()
    {
        if (rb2D.velocity.y < 0)
            return Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        
        return 0f;
    }

    private float AdjustJumpHeight()
    {
        wasPushingJumpButton = isPushingJumpButton;
        isPushingJumpButton = movementInput.jumpInput > 0;
        
        if ( !alreadyAdjustedJumpHeight 
             && !isPushingJumpButton && wasPushingJumpButton 
             && rb2D.velocity.y > 0 )
        {
            alreadyAdjustedJumpHeight = true;
            return -(rb2D.velocity.y / 2);
        }
        return 0f;
    }
}