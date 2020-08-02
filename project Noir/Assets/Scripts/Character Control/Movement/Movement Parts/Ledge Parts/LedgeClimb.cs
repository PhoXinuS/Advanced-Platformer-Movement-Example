﻿using UnityEngine;

[System.Serializable]
public class LedgeClimb
{
    [SerializeField] string animClimbingLedge = "isClimbingLedge";
    
    private int animClimbingLedgeHashed;
    private bool isClimbingLedge = false;
    private bool wasMovingUpwards = false;
    private bool wasMovingLeft = false;
    private bool wasMovingRight = false;
    private Vector2 ledgeClimbedPosition;
    
    private Rigidbody2D rb2D;
    private Animator animator;
    private IMovementInput movementInput;

    internal void Setup(Rigidbody2D rb2D, Animator animator, IMovementInput movementInput)
    {
        this.rb2D = rb2D;
        this.animator = animator;
        this.movementInput = movementInput;

        animClimbingLedgeHashed = Animator.StringToHash(animClimbingLedge);
    }

    internal void ApplyClimb(bool isDangling, bool shouldClimbLeftLedge, bool shouldClimbRightLedge, Vector2 ledgeClimbedPosition)
    {
        this.ledgeClimbedPosition = ledgeClimbedPosition;
        
        if (ShouldClimb(isDangling, shouldClimbLeftLedge, shouldClimbRightLedge))
        {
            StartLedgeClimbing();
        }

        if (isClimbingLedge)
        {
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        wasMovingUpwards = movementInput.verticalInput > 0;
        wasMovingRight = movementInput.horizontalInput > 0;
        wasMovingLeft = movementInput.horizontalInput < 0;
    }

    private bool ShouldClimb(bool isDangling, bool shouldClimbLeftLedge, bool shouldClimbRightLedge)
    {
        return isDangling 
               && !isClimbingLedge 
               && ((movementInput.verticalInput > 0 && !wasMovingUpwards)
                   || (movementInput.horizontalInput < 0 && !wasMovingLeft && shouldClimbLeftLedge)
                   || (movementInput.horizontalInput > 0 && !wasMovingRight && shouldClimbRightLedge));
    }

    private void StartLedgeClimbing()
    {
        animator.SetBool(animClimbingLedgeHashed, true);
        isClimbingLedge = true;
    }

    internal void Climbed()
    {
        rb2D.transform.position = ledgeClimbedPosition;
        animator.SetBool(animClimbingLedgeHashed, false);
        isClimbingLedge = false;
    }
}