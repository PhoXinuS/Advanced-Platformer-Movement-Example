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

    private List<bool> avaibleJumps = new List<bool>();
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
            avaibleJumps.Add(false);
        }
    }
    
    internal void ApplyVelocity(bool isGrounded)
    {
        if (!calculateVertical)
            return;
        
        wasPushingJumpButton = isPushingJumpButton;
        isPushingJumpButton = HoldingInputJump();

        float verticalVelocity = rigidBody2D.velocity.y;
        verticalVelocity += BetterFallingVelocity();
        verticalVelocity += AdjustJumpHeight();

        int jumpsCount = movementData.availableJumps;
        bool wasGrounded = isGrounded;
        if (isGrounded)
        {
            ResetJumps(jumpsCount);
        }
        AdjustTimers(isGrounded, wasGrounded);

        if (ShouldJump() && CanJump())
        {
            avaibleJumps[0] = false;
            int availableJump = CalculateAvailableJumpNumber(jumpsCount);

            if (CanGroundJump(isGrounded))
            {
                verticalVelocity = movementData.jumpHeight;
            }
            else
            {
                avaibleJumps[availableJump] = false;
                verticalVelocity = movementData.jumpHeight;
            }
        }
        
        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, verticalVelocity);
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
            avaibleJumps[j] = true;
        }
    }
    
    private void AdjustTimers(bool isGrounded, bool wasGrounded)
    {
        if (wasGrounded && !isGrounded)
            delayedJumpPressCounter = delayedJumpPressTime;

        if (isPushingJumpButton
            && !wasPushingJumpButton
            && !avaibleJumps.Contains(true))
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
        if (avaibleJumps.Contains(true))
            return true;

        return false;
    }
    
    private int CalculateAvailableJumpNumber(int jumpsCount)
    {
        for (int i = 1; i < jumpsCount; i++)
        {
            if (avaibleJumps[i])
            {
                return i;
            }
        }

        return 0;
    }
    
    private bool CanGroundJump(bool isGrounded)
    {
        return (isGrounded || delayedJumpPressCounter > 0);
    }

    private bool HoldingInputJump()
    {
        return movementInput.jumpInput > 0f;
    }
}
