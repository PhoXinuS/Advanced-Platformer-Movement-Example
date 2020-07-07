using UnityEngine;

[System.Serializable]
internal class CalculateHorizontalVelocity
{
    [SerializeField] bool calculateHorizontal = true;
    [SerializeField] Crouch crouch = new Crouch();
    
    private float xVelocity;
    private bool wasSliding;
    private bool isSliding;
    
    [SerializeField] float wallJumpControlTime = 1f;
    private float wallJumpControlCounter;
    private bool jumpedFromLeftWall;

    private MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;

    internal void Setup( MovementDataSO movementData
        , Rigidbody2D rigidBody2D
        , IMovementInput movementInput )
    {       
        this.movementData = movementData;
        this.rigidBody2D = rigidBody2D;
        this.movementInput = movementInput;
        
        crouch.Setup(movementInput, movementData, rigidBody2D);
    }
    
    internal void ApplyVelocity(bool isGrounded, bool canStand, bool isTouchingClimbableCeiling
        , bool jumped, bool isTouchingLeftWall, bool isTouchingRightWall)
    {
        if (!calculateHorizontal) return;
        
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

        rigidBody2D.velocity = new Vector2(horizontalVelocity, rigidBody2D.velocity.y);
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
                xVelocity = 0f;
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
        float smoothingTime = 0f;

        if (ContinueSliding())
        {
            smoothingTime = movementData.slideDecelerationTime;
        }
        else if (VelocityIsIncreasing(horizontalTargetVelocity))
        {
            if (isGrounded)
            {
                smoothingTime = movementData.accelerationTime;
            }
            else if (isClimbing)
            {
                smoothingTime = movementData.climbAccelerationTime;
            }
            else
            {
                smoothingTime = movementData.spaceAccelerationTime;
            }
        }
        else if(VelocityIsDecreasing(horizontalTargetVelocity))
        {
            if (isGrounded)
            {
                smoothingTime = movementData.decelerationTime;
            }
            else if (isClimbing)
            {
                smoothingTime = movementData.climbDecelerationTime;
            }
            else
            {
                smoothingTime = movementData.spaceDecelerationTime;
            }
        }

        return SmoothedVelocity(horizontalTargetVelocity, smoothingTime);
    }

    private bool ContinueSliding()
    {
        return isSliding && !InputDirectionIsOppositeToVelocity();
    }

    private bool InputDirectionIsOppositeToVelocity()
    {
        if (movementInput.horizontalInput == 0f)
            return false;

        return -Mathf.Sign(movementInput.horizontalInput) == Mathf.Sign(rigidBody2D.velocity.x);
    }
    
    private bool VelocityIsIncreasing(float horizontalTargetVelocity)
    {
        return Mathf.Abs(horizontalTargetVelocity) > Mathf.Abs(rigidBody2D.velocity.x);
    }
    private bool VelocityIsDecreasing(float horizontalTargetVelocity)
    {
        return Mathf.Abs(horizontalTargetVelocity) < Mathf.Abs(rigidBody2D.velocity.x);
    }
    
    private float SmoothedVelocity(float horizontalTargetVelocity, float smoothingTime)
    {
        float smoothedVelocity = Mathf.SmoothDamp(rigidBody2D.velocity.x, horizontalTargetVelocity, ref xVelocity, smoothingTime, Mathf.Infinity, Time.fixedDeltaTime);
        return smoothedVelocity; 
    }
    
    #endregion

    #region Wall Jump Horizontal
    private static bool WallJumped(bool isGrounded, bool jumped)
    {
        return jumped && !isGrounded;
    }

    private float ApplyWallJumpVerticalPower(bool isTouchingLeftWall,
        bool isTouchingRightWall, float horizontalVelocity)
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