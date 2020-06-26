using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
internal class CalculateHorizontalVelocity
{
    internal void Setup( MovementDataSO movementData
        , GameObject gameObject
        , Rigidbody2D rigidBody2D
        , IMovementInput movementInput )
    {       
        this.movementData = movementData;
        this.gameObject = gameObject;
        this.rigidBody2D = rigidBody2D;
        this.movementInput = movementInput;
    }
    private MovementDataSO movementData;
    private GameObject gameObject;
    private Rigidbody2D rigidBody2D;
    private IMovementInput movementInput;
    private float xVelocity;
    
    [SerializeField] bool calculateHorizontalVelocity = true;
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] Transform[] wallChecks;
    [SerializeField] float wallCheckLength = 1f;
    

    internal float Calculate()
    {
        if (!calculateHorizontalVelocity)
            return rigidBody2D.velocity.x;

        float horizontalInput = movementInput.horizontalInput;
        float horizontalTargetVelocity = horizontalInput * movementData.horizontalSpeed * TimeMultiplier();
        float horizontalVelocity = ApplyAccelerationToVelocity(horizontalTargetVelocity);
        
        if (IsRunningIntoWall(horizontalVelocity)) return 0f;
        
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

    
    private bool IsRunningIntoWall(float horizontalVelocity)
    {
        foreach (var wallCheck in wallChecks)
        {
            var hits = CastRaycastInDirectionOfMovement(horizontalVelocity, wallCheck);

            if (HittedSomethingElseThanCastingGameObject(hits)) return true;
        }        
        return false;
    }
    
    private RaycastHit2D[] CastRaycastInDirectionOfMovement(float horizontalVelocity, Transform wallCheck)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(wallCheck.position
            , Vector2.right * horizontalVelocity
            , wallCheckLength
            , whatIsWall);
        return hits;
    }
    
    private bool HittedSomethingElseThanCastingGameObject(RaycastHit2D[] hits)
    {
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
                return true;
        }

        return false;
    }
}