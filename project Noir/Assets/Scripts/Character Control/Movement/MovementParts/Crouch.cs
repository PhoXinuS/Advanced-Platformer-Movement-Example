using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Crouch
{
    [HideInInspector] public bool isCrouching;
    
    [SerializeField] bool calculateCrouch = true;
    [SerializeField] Collider2D[] normalColliders = new Collider2D[0];
    [SerializeField] Collider2D[] crouchColliders = new Collider2D[0];

    private bool wasStanding = true;

    private IMovementInput movementInput;

    internal void Setup(IMovementInput movementInput)
    {
        this.movementInput = movementInput;
    }

    internal void Calculate(bool isGrounded, bool canStand)
    {
        if (CrouchIsTriggered() && isGrounded)
        {
            isCrouching = true;
            
            foreach (var normalCollider in normalColliders)
                normalCollider.enabled = false;
            foreach (var crouchCollider in crouchColliders)
                crouchCollider.enabled = true;

            wasStanding = false;
        }
        else if(!wasStanding && canStand)
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
    
}  
