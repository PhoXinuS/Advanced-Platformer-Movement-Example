using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
internal class CalculateVerticalVelocity
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

        int jumpsCount = movementData.availableJumps;
        for (int i = 0; i < jumpsCount; i++)
        {
            avaibleJumps.Add(false);
        }
    }

    
    private MovementDataSO movementData;
    private Rigidbody2D rigidBody2D;
    private GameObject gameObject;
    private IMovementInput movementInput;
    
    [SerializeField] bool calculateVerticalVelocity = true;
    [SerializeField] Transform[] groundCheckers;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float rememberJumpPressTime = 0.15f;
    [SerializeField] float delayedJumpPressTime = 0.1f;
    [SerializeField] float fallMultiplier = 2.5f;
    
    private float rememberJumpPressCounter;
    private float delayedJumpPressCounter;

    private List<bool> avaibleJumps = new List<bool>();
    private bool grounded;
    private bool isPushingJumpButton = false;
    private bool wasPushingJumpButton;

    internal float Calculate()
    {
        //TODO: remove ground detection from this class to another marked with interface.
        //TODO: or just make another class and ScriptableObject with grounded variable

        wasPushingJumpButton = isPushingJumpButton;
        isPushingJumpButton = HoldingInputUpButton();

        float verticalVelocity = rigidBody2D.velocity.y;
        verticalVelocity += BetterFallingVelocity();
        verticalVelocity += AdjustJumpHeight();
        
        if (!calculateVerticalVelocity)
            return verticalVelocity;
        
        int jumpsCount = movementData.availableJumps;
        bool wasGrounded = grounded;
        grounded = GroundCheck.IsTouchingGround(whatIsGround, groundCheckers, gameObject);
        if (grounded)
        {
            ResetJumps(jumpsCount);
        }
        AdjustTimers(wasGrounded);

        if (ShouldJump() && CanJump())
        {
            Debug.Log(isPushingJumpButton);
            
            avaibleJumps[0] = false;
            int availableJump = CalculateAvailableJumpNumber(jumpsCount);
            
            if (CanGroundJump(availableJump))
                return movementData.jumpHeight;
                
            avaibleJumps[availableJump] = false;
            return movementData.jumpHeight;
        }


        
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
            avaibleJumps[j] = true;
        }
    }
    
    private void AdjustTimers(bool wasGrounded)
    {
        if (wasGrounded && !grounded)
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
    
    private bool CanGroundJump(int availableJump)
    {
        return (grounded || delayedJumpPressCounter > 0);
    }

    private bool HoldingInputUpButton()
    {
        return movementInput.jumpInput > 0f;
    }
}
