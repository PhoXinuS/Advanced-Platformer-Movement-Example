using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

[System.Serializable]
internal class CalculateHorizontalVelocity
{
    [SerializeField] bool calculateHorizontal = true;
    [SerializeField] Crouch crouch = new Crouch();
    [SerializeField] Slide slide = new Slide();
    
    private float xVelocity;
    
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
        
        crouch.Setup(movementInput);
        slide.Setup(movementData, rigidBody2D);
    }
    
    internal void ApplyVelocity(bool isGrounded, bool canStand)
    {
        if (!calculateHorizontal)
            return;
        
        crouch.Calculate(isGrounded, canStand);
        slide.Calculate(crouch.isCrouching);
        
        float horizontalInput = movementInput.horizontalInput;
        float horizontalTargetVelocity = horizontalInput * movementData.horizontalSpeed * TimeMultiplier();
        
        if (crouch.isCrouching)
        {
            horizontalTargetVelocity *= movementData.crouchSpeedMultiplier;
        }
        if (slide.isSliding)
        {
            horizontalTargetVelocity = 0f;
        }

        float horizontalVelocity = ApplyAccelerationToVelocity(horizontalTargetVelocity);
        rigidBody2D.velocity = new Vector2(horizontalVelocity, rigidBody2D.velocity.y);
    }
    
    private static float TimeMultiplier()
    {
        return Time.fixedDeltaTime * 100f;
    }

    private float ApplyAccelerationToVelocity(float horizontalTargetVelocity)
    {
        float accelerationTime = 0f; 
        
        if (slide.isSliding && !InputDirectionIsOppositeToVelocity())
            accelerationTime = movementData.slideDecelerationTime;
        else if (VelocityIsIncreasing(horizontalTargetVelocity))
            accelerationTime = movementData.accelerationTime;
        else
            accelerationTime = movementData.decelerationTime;
        
        return AccelerateVelocity(horizontalTargetVelocity, accelerationTime);
    }
    
    private bool InputDirectionIsOppositeToVelocity()
    {
        if (movementInput.horizontalInput == 0)
        {
            return false;
        }
        
        return -Mathf.Sign(movementInput.horizontalInput) == Mathf.Sign(rigidBody2D.velocity.x);
    }
    
    private bool VelocityIsIncreasing(float horizontalTargetVelocity)
    {
        return Mathf.Abs(horizontalTargetVelocity) > Mathf.Abs(rigidBody2D.velocity.x);
    }
    
    private float AccelerateVelocity(float horizontalTargetVelocity, float accelerationTime)
    {
        return Mathf.SmoothDamp(rigidBody2D.velocity.x, horizontalTargetVelocity, ref xVelocity, accelerationTime);
    }
}