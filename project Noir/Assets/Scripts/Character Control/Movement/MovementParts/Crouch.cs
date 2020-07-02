using UnityEngine;

[System.Serializable]
public class Crouch
{
    
    [SerializeField] bool calculateCrouch = true;
    [SerializeField] Collider2D[] normalColliders = new Collider2D[0];
    [SerializeField] Collider2D[] crouchColliders = new Collider2D[0];

    internal bool isCrouching;
    
    private bool wasStanding = true;
    private IMovementInput movementInput;

    internal void Setup(IMovementInput movementInput)
    {
        this.movementInput = movementInput;
    }

    internal void Tick(bool isGrounded, bool canStand)
    {
        if (!calculateCrouch)
        {
            isCrouching = false;
            return;
        }

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
        return movementInput.crouchInput > 0f;
    }
    
}  
