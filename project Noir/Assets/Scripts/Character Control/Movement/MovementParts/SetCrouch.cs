using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SetCrouch
{
    [SerializeField] bool calculateCrouch = true;
    [SerializeField] Collider2D[] normalColliders;
    [SerializeField] Collider2D[] crouchColliders;
    [SerializeField] Transform[] standingSpaceCheckers;
    [SerializeField] LayerMask whatPreventsStandingUp;
    [SerializeField] float standingCheckLength = 0.8f;
    
    private bool wasStanding = true;

    private CanStandCheck canStandCheck = new CanStandCheck();
    private IMovementInput movementInput;
    private GameObject gameObject;

    internal void Setup(IMovementInput movementInput, GameObject gameObject)
    {
        this.movementInput = movementInput;
        this.gameObject = gameObject;
    }

    internal void Set(ref bool isCrouching)
    {
        if (CrouchIsTriggered())
        {
            isCrouching = true;
            
            foreach (var normalCollider in normalColliders)
                normalCollider.enabled = false;
            foreach (var crouchCollider in crouchColliders)
                crouchCollider.enabled = true;


            wasStanding = false;
        }
        else if(!wasStanding && CanStandUp())
        {
            isCrouching = false;
            
            foreach (var normalCollider in normalColliders)
                normalCollider.enabled = true;
            foreach (var crouchCollider in crouchColliders)
                crouchCollider.enabled = false;


            wasStanding = true;
        }
    }

    private bool CrouchIsTriggered()
    {
        return calculateCrouch
               && movementInput.crouchInput > 0f;
    }

    private bool CanStandUp()
    {
        return canStandCheck.CanStand(whatPreventsStandingUp, standingSpaceCheckers, standingCheckLength, gameObject);
    }
}  
