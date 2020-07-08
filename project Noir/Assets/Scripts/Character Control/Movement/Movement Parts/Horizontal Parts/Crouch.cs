using UnityEngine;

[System.Serializable]
public class Crouch
{
    public Slide slide = new Slide();
    internal bool isCrouching;

    [SerializeField] Collider2D[] normalColliders = new Collider2D[0];
    [SerializeField] Collider2D[] crouchColliders = new Collider2D[0];
    
    private bool wasStanding = true;
    private IMovementInput movementInput;
    private MovementDataSO movementData;

    internal void Setup(IMovementInput movementInput
        , MovementDataSO movementData
        , Rigidbody2D rigidBody2D)
    {
        this.movementInput = movementInput;
        this.movementData = movementData;
        slide.Setup(movementData, rigidBody2D);
    }

    internal void Tick(bool isGrounded, bool canStand)
    {
        if (!movementData.calculateCrouch)
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
        
        slide.Tick(isCrouching);
    }

    private bool CrouchIsTriggered()
    {
        return movementInput.verticalInput < 0f;
    }
    
}  
