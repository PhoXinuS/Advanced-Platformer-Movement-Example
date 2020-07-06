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
    
    internal void ApplyVelocity(bool isGrounded, bool canStand
        , bool jumped, bool isTouchingLeftWall, bool isTouchingRightWall)
    {
        if (!calculateHorizontal) return;
        
        crouch.Tick(isGrounded, canStand);
        wasSliding = isSliding;
        isSliding = crouch.slide.isSliding;

        float horizontalTargetVelocity = CalculateHorizontalTargetVelocity();

        if (wallJumpControlCounter > 0)
        {
            wallJumpControlCounter -= Time.fixedDeltaTime;
            if ((jumpedFromLeftWall && horizontalTargetVelocity < 0)
                 || (!jumpedFromLeftWall && horizontalTargetVelocity > 0))
            {
                horizontalTargetVelocity = 0f;
            }

        }
        
        float horizontalVelocity = ApplySmoothnessToVelocity(horizontalTargetVelocity, isGrounded);

        if (jumped && !isGrounded)
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
        }
        
        rigidBody2D.velocity = new Vector2(horizontalVelocity, rigidBody2D.velocity.y);
    }
    
    #region Calculate Target Velocity
    private float CalculateHorizontalTargetVelocity()
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

        return horizontalTargetVelocity;
    }

    #endregion
    
    #region Smoothen Velocity
    private float ApplySmoothnessToVelocity(float horizontalTargetVelocity, bool isGrounded)
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
    
}