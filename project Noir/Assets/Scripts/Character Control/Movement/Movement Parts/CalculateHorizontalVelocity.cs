using UnityEngine;

[System.Serializable]
internal class CalculateHorizontalVelocity
{
    [SerializeField] Crouch crouch = new Crouch();
    
    private bool wasSliding;
    private bool isSliding;
    
    [SerializeField] float wallJumpControlTime = 1f;
    private float wallJumpControlCounter;
    private bool jumpedFromLeftWall;

    private MovementDataSO movementData;
    private Rigidbody2D rb2D;
    private IMovementInput movementInput;
    
    private VelocitySmoother velocitySmoother = new VelocitySmoother();

    internal void Setup( MovementDataSO movementData
        , Rigidbody2D rb2D
        , Animator animator
        , IMovementInput movementInput )
    {       
        this.movementData = movementData;
        this.rb2D = rb2D;
        this.movementInput = movementInput;
        
        crouch.Setup(movementInput, movementData, rb2D, animator);
    }

    internal bool IsCrouching()
    {
        return crouch.isCrouching;
    }
    
    internal void ApplyVelocity(bool isGrounded, bool canStand, bool isTouchingClimbableCeiling
        , bool jumped, bool isTouchingLeftWall, bool isTouchingRightWall)
    {
        if (!movementData.calculateHorizontal) return;
        
        crouch.Tick(isGrounded, canStand);
        bool isCrouching = crouch.isCrouching;
        wasSliding = isSliding;
        isSliding = crouch.slide.isSliding;

        float horizontalTargetVelocity = CalculateHorizontalTargetVelocity(isTouchingClimbableCeiling);
        float horizontalVelocity = ApplySmoothnessToVelocity(horizontalTargetVelocity, isGrounded, isTouchingClimbableCeiling);
        
        if (WallJumped(isGrounded, jumped))
        {
            horizontalVelocity = ApplyWallJumpVerticalPower(isTouchingLeftWall, isTouchingRightWall, horizontalVelocity);
        }
        else if ((isTouchingLeftWall || isTouchingRightWall)
                 && (!isCrouching || !isSliding) 
                 && movementInput.horizontalInput == 0)
        {
            horizontalVelocity = 0f;    // stop on wall, prevents from bouncing off
        }

        rb2D.velocity = new Vector2(horizontalVelocity, rb2D.velocity.y);
    }

    #region Calculate Target Velocity
    private float CalculateHorizontalTargetVelocity(bool isTouchingClimbableCeiling)
    {
        float horizontalInput = movementInput.horizontalInput;
        float horizontalTargetVelocity = horizontalInput * movementData.horizontalSpeed;

        if (crouch.isCrouching)
        {
            horizontalTargetVelocity *= movementData.crouchSpeedMultiplier;
        }

        if (isSliding)
        {
            horizontalTargetVelocity = 0f;
            if (!wasSliding)
            {
                velocitySmoother.xVelocity = 0f;
            }
        }

        if (isTouchingClimbableCeiling)
        {
            horizontalTargetVelocity = horizontalInput * movementData.ceilingClimbSpeed;
        }
        
        if (AfterWallJumpTimerIsActive())
        {
            horizontalTargetVelocity = DisableDirectionTowardsWallAfterWallJump(horizontalTargetVelocity);
        }

        return horizontalTargetVelocity;
    }

    private bool AfterWallJumpTimerIsActive()
    {
        return wallJumpControlCounter > 0;
    }

    private float DisableDirectionTowardsWallAfterWallJump(float horizontalTargetVelocity)
    {
        wallJumpControlCounter -= Time.fixedDeltaTime;
        if ((jumpedFromLeftWall && horizontalTargetVelocity < 0)
            || (!jumpedFromLeftWall && horizontalTargetVelocity > 0))
        {
            return 0f;
        }

        return horizontalTargetVelocity;
    }

    #endregion
    
    #region Smoothen Velocity
    private float ApplySmoothnessToVelocity(float horizontalTargetVelocity, bool isGrounded, bool isClimbing)
    {
        float accelerationTime = 0f;
        float decelerationTime = 0f;

        if (ContinueSliding())
        {
            decelerationTime = movementData.slideDecelerationTime;
        }
        else if (isGrounded)
        {
            accelerationTime = movementData.accelerationTime;     
            decelerationTime = movementData.decelerationTime;

        }
        else if (isClimbing)
        {
            accelerationTime = movementData.climbAccelerationTime;    
            decelerationTime = movementData.climbDecelerationTime;

        }
        else
        {
            accelerationTime = movementData.spaceAccelerationTime;
            decelerationTime = movementData.spaceDecelerationTime;
        }
            
        return velocitySmoother.SmoothedVelocity(horizontalTargetVelocity, rb2D.velocity.x, accelerationTime, decelerationTime);
    }

    private bool ContinueSliding()
    {
        return isSliding && !InputDirectionIsOppositeToVelocity();
    }

    private bool InputDirectionIsOppositeToVelocity()
    {
        if (movementInput.horizontalInput == 0f)
            return false;

        return -Mathf.Sign(movementInput.horizontalInput) == Mathf.Sign(rb2D.velocity.x);
    }

    #endregion

    #region Wall Jump Horizontal
    private static bool WallJumped(bool isGrounded, bool jumped)
    {
        return jumped && !isGrounded;
    }

    private float ApplyWallJumpVerticalPower(bool isTouchingLeftWall, bool isTouchingRightWall, float horizontalVelocity)
    {
        
        if (isTouchingLeftWall)
        {
            horizontalVelocity = movementData.wallJumpHorizontalPower;
            wallJumpControlCounter = wallJumpControlTime;
            jumpedFromLeftWall = true;
        }
        else if (isTouchingRightWall)
        {
            horizontalVelocity = -movementData.wallJumpHorizontalPower;
            wallJumpControlCounter = wallJumpControlTime;
            jumpedFromLeftWall = false;
        }
        

        return horizontalVelocity;
    }
    
    #endregion
    
}