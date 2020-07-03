using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
internal class CalculateVerticalVelocity
{
    [SerializeField] bool calculateVertical = true;
    [SerializeField] float rememberJumpPressTime = 0.15f;
    [SerializeField] float delayedJumpPressTime = 0.1f;
    [SerializeField] float fallMultiplier = 2.5f;
    
    private float rememberJumpPressCounter;
    private float delayedJumpPressCounter;

    private List<bool> availableJumps = new List<bool>();
    private bool isPushingJumpButton = false;
    private bool wasPushingJumpButton;
    
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

        int jumpsCount = movementData.availableJumps;
        for (int i = 0; i < jumpsCount; i++)
        {
            availableJumps.Add(false);
        }
    }
    
    internal void ApplyVelocity(bool isGrounded, bool canStand
        , bool isTouchingWall, bool isTouchingClimbableWall)
    {
        rigidBody2D.gravityScale = 1f;
        if (!calculateVertical || !canStand) return;
        
        wasPushingJumpButton = isPushingJumpButton;
        isPushingJumpButton = HoldingInputJump();

        var verticalVelocity = ApplyVerticalAdjusters();

        if (isTouchingWall && !isTouchingClimbableWall && rigidBody2D.velocity.y < 0f)
        {
            verticalVelocity = -movementData.wallSlideSpeed;
        }
        
        bool wasGrounded = isGrounded;
        if (isGrounded || isTouchingWall)
        {
            ResetJumps(movementData.availableJumps);
        }
        AdjustTimers(isGrounded, wasGrounded);

        if (ShouldJump() && CanJump())
        {
            UseSingleJump();
            if (!CanGroundJump(isGrounded))
            {
                UseMiltipleJump();
            }
            verticalVelocity = movementData.jumpHeight;
        }
        
        if (isTouchingClimbableWall && rigidBody2D.velocity.y < 0f)
        {
            rigidBody2D.gravityScale = 0f;
            verticalVelocity = movementInput.verticalInput * movementData.wallClimbSpeed;
        }
        
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, verticalVelocity);
    }

    private bool HoldingInputJump()
    {
        return movementInput.jumpInput > 0f;
    }

    private float ApplyVerticalAdjusters()
    {
        float verticalVelocity = rigidBody2D.velocity.y;
        verticalVelocity += BetterFallingVelocity();
        verticalVelocity += AdjustJumpHeight();
        return verticalVelocity;
    }

    private float BetterFallingVelocity()
    {
        if (rigidBody2D.velocity.y < 0)
            return Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        
        return 0f;
    }
    
    private float AdjustJumpHeight()
    {
        if (!isPushingJumpButton
            && wasPushingJumpButton
            && rigidBody2D.velocity.y > 0)
        {
            return -(rigidBody2D.velocity.y / 2);
        }
        return 0f;
    }
    
    private void ResetJumps(int jumpsCount)
    {
        for (int j = 0; j < jumpsCount; j++)
        {
            availableJumps[j] = true;
        }
    }
    
    private void AdjustTimers(bool isGrounded, bool wasGrounded)
    {
        if (wasGrounded && !isGrounded)
            delayedJumpPressCounter = delayedJumpPressTime;

        if (isPushingJumpButton
            && !wasPushingJumpButton
            && !availableJumps.Contains(true))
        {
            rememberJumpPressCounter = rememberJumpPressTime;
        }

        if (delayedJumpPressCounter > 0)
            delayedJumpPressCounter -= Time.fixedDeltaTime;
        if (rememberJumpPressCounter > 0)
            rememberJumpPressCounter -= Time.fixedDeltaTime;
    }
    
    private bool ShouldJump()
    {
        return isPushingJumpButton && !wasPushingJumpButton
               || rememberJumpPressCounter > 0;

    }
    private bool CanJump()
    {
        if (availableJumps.Contains(true)) return true;
        
        return false;
    }
    
    private void UseSingleJump()
    {
        availableJumps[0] = false; // No matter if object is grounded or not first (ground) jump should be set as used.
    }
    
    private bool CanGroundJump(bool isGrounded)
    {
        return (isGrounded || delayedJumpPressCounter > 0);
    }
    
    private void UseMiltipleJump()
    {
        int availableJump = CalculateAvailableJumpNumber(movementData.availableJumps);
        availableJumps[availableJump] = false;
    }
    
    private int CalculateAvailableJumpNumber(int jumpsCount)
    {
        for (int i = 1; i < jumpsCount; i++)
        {
            if (availableJumps[i]) return i;
        }
        return 0;
    }
}
