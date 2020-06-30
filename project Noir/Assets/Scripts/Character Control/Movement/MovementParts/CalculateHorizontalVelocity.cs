using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
internal class CalculateHorizontalVelocity
{
    [SerializeField] bool calculateHorizontal = true;

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
    }
    
    internal float Calculate(bool isCrouching)
    {
        if (!calculateHorizontal)
            return rigidBody2D.velocity.x;
        
    
        float horizontalInput = movementInput.horizontalInput;
        float horizontalTargetVelocity = horizontalInput * movementData.horizontalSpeed * TimeMultiplier();
        
        if (isCrouching)
        {
            horizontalTargetVelocity *= movementData.crouchSpeedMultiplier;
        }

        float horizontalVelocity = ApplyAccelerationToVelocity(horizontalTargetVelocity);

        return horizontalVelocity;
    }
    
     
    private static float TimeMultiplier()
    {
        return Time.fixedDeltaTime * 100f;
    }

    private float ApplyAccelerationToVelocity(float horizontalTargetVelocity)
    {
        if (VelocityIsIncreasing(horizontalTargetVelocity))
            return AccelerateVelocity(horizontalTargetVelocity, movementData.accelerationTime);

        return AccelerateVelocity(horizontalTargetVelocity, movementData.decelerationTime);
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