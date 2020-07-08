﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[System.Serializable]
internal class CalculateVerticalVelocity
{
    [SerializeField] float rememberJumpPressTime = 0.15f;
    [SerializeField] float delayedJumpPressTime = 0.1f;
    
    private float rememberJumpPressCounter;
    private float delayedJumpPressCounter;

    private List<bool> availableJumps = new List<bool>();
    private bool isPushingJumpButton;
    private bool wasPushingJumpButton;
    
    private MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    
    private VerticalAdjusters verticalAdjusters = new VerticalAdjusters();
    private VelocitySmoother velocitySmoother = new VelocitySmoother();

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
        
        verticalAdjusters.SetUp(rigidBody2D, movementInput);
    }
    
    internal void ApplyVelocity(bool isGrounded, bool canStand, bool isTouchingClimbableCeiling
        , bool isTouchingLeftWall, bool isTouchingRightWall, bool isTouchingClimbableWall
        , ref bool jumped)
    {
        rigidBody2D.gravityScale = 1f;
        if (!movementData.calculateVertical || !canStand) return;
        
        wasPushingJumpButton = isPushingJumpButton;
        isPushingJumpButton = HoldingInputJump();

        var velocity = rigidBody2D.velocity;
        float horizontalVelocity = velocity.x;
        float verticalVelocity = velocity.y;
        bool isTouchingWall = isTouchingRightWall || isTouchingLeftWall;

        if (isTouchingWall && !isTouchingClimbableWall && rigidBody2D.velocity.y < 0f)
        {
            verticalVelocity = -movementData.wallSlideSpeed;
        }
        verticalVelocity = ApplyWallClimb(isTouchingClimbableWall, verticalVelocity);

        
        bool wasGrounded = isGrounded;
        if (isGrounded || isTouchingClimbableWall || isTouchingClimbableCeiling)
        {
            ResetJumps(movementData.availableJumps);
        }
        else if (isTouchingWall)
        {
            ResetJumps(1);
        }
        AdjustTimers(isGrounded, wasGrounded);

        bool adjustJumpHeight = true;
        if (ShouldJump() && CanJump() && !isTouchingClimbableCeiling)
        {
            UseSingleJump();
            if (!CanGroundJump(isGrounded))
            {
                UseMiltipleJump();
            }

            if (!isTouchingWall)
            {
                verticalVelocity = movementData.jumpHeight;
            }
            else
            {
                verticalVelocity = movementData.wallJumpVerticalPower;   
                adjustJumpHeight = false;
            }
            jumped = true;
        }

        if (!IsSliding(isTouchingWall, isGrounded)
            && !IsClimbing(isTouchingClimbableWall, isGrounded))
        {
            verticalVelocity = verticalAdjusters.ApplyAdjusters(verticalVelocity, jumped, true, adjustJumpHeight);
        }
        verticalVelocity = ApplyCeilingClimb(isTouchingClimbableCeiling, verticalVelocity);
        rigidBody2D.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }
    
    
    private bool HoldingInputJump()
    {
        return movementInput.jumpInput > 0f;
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
    
    private bool CanGroundJump(bool isGrounded)
    {
        return (isGrounded || delayedJumpPressCounter > 0);
    }
    
    private bool IsClimbing(bool isTouchingClimbableObject, bool isGrounded)
    {
        return movementData.calculateClimb
               && isTouchingClimbableObject 
               && !isGrounded;
    }

    private bool IsSliding(bool isTouchingSlidableObject, bool isGrounded)
    {
        return movementData.calculateSlide
               && isTouchingSlidableObject 
               && !isGrounded;
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
    
    private void UseSingleJump()
    {
        availableJumps[0] = false; // No matter if object is grounded or not first (ground) jump should be set as used.
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


    private float ApplyWallClimb(bool isTouchingClimbableWall, float verticalVelocity)
    {
        if (isTouchingClimbableWall)
        {
            rigidBody2D.gravityScale = 0f;
            float targetVelocity = movementInput.verticalInput * movementData.wallClimbSpeed;
            float currentVelocity = rigidBody2D.velocity.y;
            float accelerationTime = movementData.climbAccelerationTime;
            float decelerationTime = movementData.climbDecelerationTime;
            
            verticalVelocity = velocitySmoother.SmoothedVelocity(targetVelocity, currentVelocity, accelerationTime, decelerationTime);
        }
        
        return verticalVelocity;
    }

    private float ApplyCeilingClimb(bool isTouchingClimbableCeiling, float verticalVelocity)
    {
        if (isTouchingClimbableCeiling && movementInput.verticalInput >= 0)
        {
            verticalVelocity = 0f;
            rigidBody2D.gravityScale = 0f;
        }

        return verticalVelocity;
    }
}
